// Services/Features/Cryptography/EncryptionService.cs
using System.Text;

// Asegúrate de que el namespace coincida con el de tu proyecto
namespace ENCRYPT.Services.Features.Cryptography 
{
    public class EncryptionService
    {
        private const int Shift = 5;
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        public string Encrypt(string plainText)
        {
            var encryptedText = new StringBuilder();

            foreach (char character in plainText)
            {
                if (char.IsLetter(character))
                {
                    char charLower = char.ToLower(character);
                    int index = Alphabet.IndexOf(charLower);

                    if (index != -1) // Si es una letra del alfabeto simple (a-z)
                    {
                        int newIndex = (index + Shift) % Alphabet.Length;
                        char newChar = Alphabet[newIndex];
                        
                        // Mantenemos si era mayúscula o minúscula originalmente
                        encryptedText.Append(char.IsUpper(character) ? char.ToUpper(newChar) : newChar);
                    }
                    else
                    {
                        // Para caracteres con acentos o especiales que IsLetter considera letra (ñ, á, etc.)
                        encryptedText.Append(character);
                    }
                }
                else
                {
                    // No es una letra (números, espacios, signos), la dejamos como está
                    encryptedText.Append(character);
                }
            }
            return encryptedText.ToString();
        }

        public string Decrypt(string encryptedText)
        {
            var plainText = new StringBuilder();

            foreach (char character in encryptedText)
            {
                if (char.IsLetter(character))
                {
                    char charLower = char.ToLower(character);
                    int index = Alphabet.IndexOf(charLower);

                    if (index != -1)
                    {
                        // La fórmula para desencriptar
                        int newIndex = (index - Shift + Alphabet.Length) % Alphabet.Length;
                        char newChar = Alphabet[newIndex];
                        plainText.Append(char.IsUpper(character) ? char.ToUpper(newChar) : newChar);
                    }
                    else
                    {
                        plainText.Append(character);
                    }
                }
                else
                {
                    plainText.Append(character);
                }
            }
            return plainText.ToString();
        }
    }
}