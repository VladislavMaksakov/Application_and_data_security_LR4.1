using System.Security.Cryptography;
using System.Text;


Console.Write("Введіть ім'я: ");
string name = Console.ReadLine() ?? string.Empty;
Console.Write("Дата народження: ");
string dob = Console.ReadLine() ?? string.Empty;
Console.Write("Секретне слово: ");
string secret = Console.ReadLine() ?? string.Empty;

string CalculateHash(string input)
{
   using var sha256 = SHA256.Create();
   byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
   return Convert.ToHexString(bytes);
}

string privateKey = CalculateHash($"{name}{dob}{secret}");
string publicKey = CalculateHash(privateKey);

await File.WriteAllTextAsync("private_key.txt", privateKey);
await File.WriteAllTextAsync("public_key.txt", publicKey);

Console.WriteLine($"\nУспішно!\nPrivate Key: {privateKey[..10]}...\nPublic Key:  {publicKey[..10]}...");
Console.WriteLine("Ключі збережено у папці з програмою.");