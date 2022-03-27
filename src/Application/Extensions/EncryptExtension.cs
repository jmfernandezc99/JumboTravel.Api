namespace JumboTravel.Api.src.Application.Extensions
{
    public class EncryptExtension
    {
        public static string Encrypt(int id)
        {
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(id.ToString());
            return Convert.ToBase64String(encryted);
        }
        public static int Decrypt(string id)
        {
            byte[] decryted = Convert.FromBase64String(id);
            string decryptedId = System.Text.Encoding.Unicode.GetString(decryted);
            return int.TryParse(decryptedId, out int result) ? result : 0;
        }
    }
}
