using System.Text;
using Newtonsoft.Json;

const string clientId = "D7cHX3vp7UkEhN5CJ9Rm"; // 본인의 클라이언트 아이디로 대체
const string clientSecret = "xIQGWmsRYh"; // 본인의 클라이언트 시크릿으로 대체

string sourceLang = "en";
string targetLang = "ko";
string textToTranslate = "hi";

var rootDir = args[0];

SearchFiles(rootDir);

async Task SearchFiles(string path)
{
    var files = Directory.GetFiles(path);
    var dirs = Directory.GetDirectories(path);
    Console.WriteLine($"now path : {path}");
    if (files.Length <= 0 && dirs.Length <= 0)
    {
        Console.WriteLine("end files");
        return;
    }
    foreach (var f in files)
    {
        var fileName = Path.GetFileNameWithoutExtension(f);
        Console.WriteLine($"원본 제목 : {fileName}");
    }
    
    foreach (var f in files)
    {
        var fileName = Path.GetFileNameWithoutExtension(f);
        if (fileName.Length <= 0)
        {
            continue;
        }
        var rename = await TransformText(fileName);
        Console.WriteLine($"변역 제목 : {rename}");
    }

    foreach (var dir in dirs)
    {
        await SearchFiles(dir);
    }
}





async Task<string> TransformText(string text)
{
    using (HttpClient client = new HttpClient())
    {
        StringContent content = setupParameter(sourceLang, targetLang, text, client);
        HttpResponseMessage response = await client.PostAsync("https://openapi.naver.com/v1/papago/n2mt", content);
        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            TranslationResult translationResult = JsonConvert.DeserializeObject<TranslationResult>(jsonString);
            // Console.WriteLine($"Translated Text: {translationResult.Message.Result.TranslatedText}");
            return translationResult.Message.Result.TranslatedText;
            
        } else
        {
            // Console.WriteLine("Error: " + response.StatusCode);
            throw new Exception("Error: " + response.StatusCode);
        }
    }
}

while (true)
{
}

// using (HttpClient client = new HttpClient())
// {
//     StringContent content = setupParameter(sourceLang, targetLang, textToTranslate, client);
//     HttpResponseMessage response = await client.PostAsync("https://openapi.naver.com/v1/papago/n2mt", content);
//     if (response.IsSuccessStatusCode)
//     {
//         string jsonString = await response.Content.ReadAsStringAsync();
//         TranslationResult translationResult = JsonConvert.DeserializeObject<TranslationResult>(jsonString);
//         Console.WriteLine($"Translated Text: {translationResult.Message.Result.TranslatedText}");
//         
//         Console.WriteLine(jsonString);
//     } else
//     {
//         Console.WriteLine("Error: " + response.StatusCode);
//     }
// }

StringContent setupParameter(string sourceLang, string targetLang, string textToTranslate, HttpClient client)
{
    client.DefaultRequestHeaders.Add("X-Naver-Client-Id", clientId);
    client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", clientSecret);

    var content = new StringContent($"source={sourceLang}&target={targetLang}&text={textToTranslate}", Encoding.UTF8, "application/x-www-form-urlencoded");
    return content;
}

public class TranslationResult
{
    [JsonProperty("message")]
    public Message Message { get; set; }
}

public class Message
{
    public Result Result { get; set; }
}

public class Result
{
    [JsonProperty("translatedText")]
    public string TranslatedText { get; set; }
}