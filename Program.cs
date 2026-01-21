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

Console.WriteLine("--- Підписання документів ---");

string docPath = "contract.txt";
string originalContent = "Я заповідаю все майно коту.";
await File.WriteAllTextAsync(docPath, originalContent);
Console.WriteLine($"[1] Документ створено: {docPath}");
using RSA rsa = RSA.Create();
var privateKeyBytes = rsa.ExportRSAPrivateKey();
var publicKeyBytes = rsa.ExportRSAPublicKey();
byte[] dataToSign = await File.ReadAllBytesAsync(docPath);
byte[] signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
string sigPath = "contract.sig";
await File.WriteAllBytesAsync(sigPath, signature);
Console.WriteLine($"[2] Документ підписано. Підпис збережено у: {sigPath}");
bool isValid = rsa.VerifyData(dataToSign, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
Console.WriteLine($"[3] Первинна перевірка підпису: {(isValid ? "✅ ВІРНО" : "❌ ПОМИЛКА")}");

Console.WriteLine("\n--- Тестування підробки ---");
await File.AppendAllTextAsync(docPath, "\nP.S. І квартиру сусіду.");
Console.WriteLine("[!] УВАГА: Документ було непомітно змінено зловмисником!");
byte[] tamperedData = await File.ReadAllBytesAsync(docPath);
byte[] loadedSignature = await File.ReadAllBytesAsync(sigPath);
bool isTamperedValid = rsa.VerifyData(tamperedData, loadedSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
Console.WriteLine($"Result перевірки модифікованого файлу: {(isTamperedValid ? "✅ ВІРНО" : "❌ ВИЯВЛЕНО ПІДРОБКУ")}");
Console.WriteLine("[Info] Спроба згенерувати новий підпис без приватного ключа неможлива математично.");