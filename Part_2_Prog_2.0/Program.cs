using System;
using System.Collections.Generic;

// Enum for food groups
enum FoodGroup
{
    Dairy,
    Fruit,
    Grain,
    Protein,
    Vegetable
}

// Ingredient class representing an ingredient with name, calories, and food group
class Ingredient
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public FoodGroup FoodGroup { get; set; }
}

// Recipe class representing a recipe with name, ingredients, and steps
class Recipe
{
    public string Name { get; set; }
    public List<Ingredient> Ingredients { get; set; }
    public List<string> Steps { get; set; }

    public int GetTotalCalories()
    {
        int totalCalories = 0;
        foreach (var ingredient in Ingredients)
        {
            totalCalories += ingredient.Calories;
        }
        return totalCalories;
    }
}

// RecipeManager class responsible for managing recipes
class RecipeManager
{
    private List<Recipe> recipes = new List<Recipe>();

    // Event delegate to notify when recipe exceeds 300 calories
    public delegate void RecipeCaloriesExceededHandler(Recipe recipe);

    // Event to be raised when recipe exceeds 300 calories
    public event RecipeCaloriesExceededHandler RecipeCaloriesExceeded;

    public void AddRecipe()
    {
        Recipe recipe = new Recipe();

        Console.WriteLine("Enter recipe name:");
        recipe.Name = Console.ReadLine();

        recipe.Ingredients = new List<Ingredient>();
        Console.WriteLine("Enter ingredients for the recipe (press Enter to finish):");
        while (true)
        {
            Ingredient ingredient = new Ingredient();

            Console.WriteLine("Enter ingredient name:");
            ingredient.Name = Console.ReadLine();

            if (string.IsNullOrEmpty(ingredient.Name))
                break;

            Console.WriteLine("Enter calories for the ingredient:");
            ingredient.Calories = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter food group for the ingredient (Dairy, Fruit, Grain, Protein, Vegetable):");
            ingredient.FoodGroup = Enum.Parse<FoodGroup>(Console.ReadLine(), ignoreCase: true);

            recipe.Ingredients.Add(ingredient);
        }

        recipe.Steps = new List<string>();
        Console.WriteLine("Enter recipe steps (press Enter to finish):");
        while (true)
        {
            string step = Console.ReadLine();

            if (string.IsNullOrEmpty(step))
                break;

            recipe.Steps.Add(step);
        }

        recipes.Add(recipe);

        if (recipe.GetTotalCalories() > 300)
        {
            // Raise the RecipeCaloriesExceeded event
            RecipeCaloriesExceeded?.Invoke(recipe);
        }
    }

    public void DisplayRecipes()
    {
        recipes.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
        foreach (var recipe in recipes)
        {
            Console.WriteLine(recipe.Name);
        }
    }

    public void DisplayRecipe(string recipeName)
    {
        Recipe recipe = recipes.Find(r => r.Name == recipeName);
        if (recipe != null)
        {
            Console.WriteLine("Recipe Name: " + recipe.Name);
            Console.WriteLine("Ingredients:");
            foreach (var ingredient in recipe.Ingredients)
            {
                Console.WriteLine("Name: " + ingredient.Name);
                Console.WriteLine("Calories: " + ingredient.Calories);
                Console.WriteLine("Food Group: " + ingredient.FoodGroup);
            }
            Console.WriteLine("Steps:");
            foreach (var step in recipe.Steps)
            {
                Console.WriteLine(step);
            }
            Console.WriteLine("Total Calories: " + recipe.GetTotalCalories());
        }
        else
        {
            Console.WriteLine("Recipe not found.");
        }
    }
}

// Main program
class Program
{
    static void Main(string[] args)
    {
        RecipeManager recipeManager = new RecipeManager();

        // Subscribe to the RecipeCaloriesExceeded event
        recipeManager.RecipeCaloriesExceeded += RecipeCaloriesExceededHandler;

        while (true)
        {
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Add a recipe");
            Console.WriteLine("2. Display all recipes");
            Console.WriteLine("3. Display a recipe");
            Console.WriteLine("4. Exit");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    recipeManager.AddRecipe();
                    break;
                case "2":
                    recipeManager.DisplayRecipes();
                    break;
                case "3":
                    Console.WriteLine("Enter recipe name:");
                    string recipeName = Console.ReadLine();
                    recipeManager.DisplayRecipe(recipeName);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    static void RecipeCaloriesExceededHandler(Recipe recipe)
    {
        Console.WriteLine("Warning: The total calories of recipe '" + recipe.Name + "' exceed 300.");
    }
}