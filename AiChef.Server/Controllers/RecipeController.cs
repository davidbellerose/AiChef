using AiChef.Server.Data;
using AiChef.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AiChef.Server.Services;

namespace AiChef.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IOpenAIAPI _openAiService;

        public RecipeController(IOpenAIAPI openAiService)
        {
            _openAiService = openAiService;
        }


        [HttpPost, Route("GetRecipeIdeas")]
        public async Task<ActionResult<List<Idea>>> GetRecipeIdeas(RecipeParms recipeParms)
        {
            string mealtime = recipeParms.MealTime!;
            List<string> ingredients = recipeParms.Ingredients!
                .Where(x => !string.IsNullOrEmpty(x.Description!))
                .Select(x => x.Description!)
                .ToList();

            if (string.IsNullOrEmpty(mealtime))
            {
                mealtime = "Breakfast";
            }

            var ideas = await _openAiService.CreateRecipeIdeas(mealtime, ingredients);
            return ideas;

            // use this for testing to minimize charges on api
            //return SampleData.RecipeIdeas;
        }

        [HttpPost, Route("GetRecipe")]
        public async Task<ActionResult<Recipe?>> GetRecipe(RecipeParms recipeParms)
        {
            List<string> ingredients = recipeParms.Ingredients
                        .Where(x => !string.IsNullOrEmpty(x.Description))
                        .Select(x => x.Description!)
                        .ToList();
            string? title = recipeParms.SelectedIdea;

            if (string.IsNullOrEmpty(title))
            {
                return BadRequest();
            }

            var recipe = await _openAiService.CreateRecipe(title, ingredients);
            return recipe;

            //return SampleData.Recipe;
        }


        [HttpGet, Route("GetRecipeImage")]
        public async Task<RecipeImage> GetRecipeImage(string title)
        {
            //return SampleData.RecipeImage;

            var recipeImage = await _openAiService.CreateRecipeImage(title);
            return recipeImage ?? SampleData.RecipeImage;
        }
    }
}
