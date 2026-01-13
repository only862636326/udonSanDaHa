using System;

public class TestShufflePattern
{
    public static void Main()
    {
        // Test the shuffle pattern calculation
        int[] newValues = new int[84];
        newValues[0] = 0;
        
        for (int i = 1; i < 84; i++)
        {
            newValues[i] = newValues[i - 1] + (i + 2);
        }
        
        // Print the first 10 values to verify the pattern
        Console.WriteLine("First 10 values of the shuffle pattern:");
        for (int i = 0; i < Math.Min(10, newValues.Length); i++)
        {
            Console.WriteLine($"Index {i}: Value {newValues[i]}");
        }
    }
}