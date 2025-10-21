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

// Ingredient class
class Ingredient
{
    public string Name { get; set; }
    public int Calories { get; set; }
    public FoodGroup FoodGroup { get; set; }
}

// Recipe class
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

// RecipeManager class
class RecipeManager
{
    private List<Recipe> recipes = new List<Recipe>();

    public delegate void RecipeCaloriesExceededHandler(Recipe recipe);
    public event RecipeCaloriesExceededHandler RecipeCaloriesExceeded;

    public void AddRecipe()
    {
        Recipe recipe = new Recipe();

        Console.Write("Enter recipe name: ");
        recipe.Name = Console.ReadLine();

        // Check for duplicate name
        if (recipes.Exists(r => r.Name.Equals(recipe.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("A recipe with this name already exists. Try another name.");
            return;
        }

        recipe.Ingredients = new List<Ingredient>();
        Console.WriteLine("\nEnter ingredients for the recipe (press Enter to finish):");
        while (true)
        {
            Console.Write("Enter ingredient name: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                break;

            Ingredient ingredient = new Ingredient { Name = name };

            // Input validation for calories
            int calories;
            while (true)
            {
                Console.Write("Enter calories: ");
                if (int.TryParse(Console.ReadLine(), out calories) && calories >= 0)
                {
                    ingredient.Calories = calories;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a positive number.");
                }
            }

            // Input validation for food group
            while (true)
            {
                Console.Write("Enter food group (Dairy, Fruit, Grain, Protein, Vegetable): ");
                if (Enum.TryParse<FoodGroup>(Console.ReadLine(), true, out var foodGroup))
                {
                    ingredient.FoodGroup = foodGroup;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid food group. Try again.");
                }
            }

            recipe.Ingredients.Add(ingredient);
        }

        recipe.Steps = new List<string>();
        Console.WriteLine("\nEnter recipe steps (press Enter to finish):");
        while (true)
        {
            Console.Write("Step: ");
            string step = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(step))
                break;

            recipe.Steps.Add(step);
        }

        recipes.Add(recipe);

        int totalCalories = recipe.GetTotalCalories();
        if (totalCalories > 300)
        {
            RecipeCaloriesExceeded?.Invoke(recipe);
        }

        Console.WriteLine($"\nRecipe '{recipe.Name}' added successfully! Total Calories: {totalCalories}");
    }

    public void DisplayRecipes()
    {
        if (recipes.Count == 0)
        {
            Console.WriteLine("No recipes to display.");
            return;
        }

        recipes.Sort((r1, r2) => r1.Name.CompareTo(r2.Name));
        Console.WriteLine("\n--- All Recipes ---");
        foreach (var recipe in recipes)
        {
            Console.WriteLine("- " + recipe.Name);
        }
    }

    public void DisplayRecipe(string recipeName)
    {
        Recipe recipe = recipes.Find(r => r.Name.Equals(recipeName, StringComparison.OrdinalIgnoreCase));
        if (recipe != null)
        {
            Console.WriteLine($"\n--- {recipe.Name} ---");
            Console.WriteLine("Ingredients:");
            foreach (var ingredient in recipe.Ingredients)
            {
                Console.WriteLine($"- {ingredient.Name} ({ingredient.Calories} cal, {ingredient.FoodGroup})");
            }

            Console.WriteLine("\nSteps:");
            for (int i = 0; i < recipe.Steps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {recipe.Steps[i]}");
            }

            Console.WriteLine($"\nTotal Calories: {recipe.GetTotalCalories()}");
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
    static void Main()
    {
        RecipeManager recipeManager = new RecipeManager();
        recipeManager.RecipeCaloriesExceeded += RecipeCaloriesExceededHandler;

        while (true)
        {
            Console.WriteLine("\n--- Recipe Manager ---");
            Console.WriteLine("1. Add a recipe");
            Console.WriteLine("2. Display all recipes");
            Console.WriteLine("3. Display a recipe");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option (1-4): ");
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
                    Console.Write("Enter recipe name: ");
                    string recipeName = Console.ReadLine();
                    recipeManager.DisplayRecipe(recipeName);
                    break;
                case "4":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Please select from 1 to 4.");
                    break;
            }
        }
    }

    static void RecipeCaloriesExceededHandler(Recipe recipe)
    {
        Console.WriteLine($"\n⚠️ Warning: The total calories of recipe '{recipe.Name}' exceed 300.");
    }
}
