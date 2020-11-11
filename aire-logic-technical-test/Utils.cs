using System;
using System.Linq;

namespace aire_logic_technical_test
{
    static class Utils
    {
        public static char GetInput(char[] allowableInputs)
        {
            char response = '0';
            while (!allowableInputs.Select(char.ToLower).Contains(response))
            {
                response = char.ToLower(Console.ReadKey(true).KeyChar);
            }

            return response;
        }
    }
}