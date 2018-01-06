string responseString = string.Empty;

var query = “hi”; //User Query
var knowledgebaseId = “2b1f9313-3c41-479f-b5df-65757c376f1b”; // Use knowledge base id created.
var qnamakerSubscriptionKey = “d1ef0fbb784145a6b89694bc1feeb431”; //Use subscription key assigned to you.

//Build the URI
Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

//Add the question as part of the body
var postBody = $"{{\"question\": \"{query}\"}}";

//Send the POST request
using (WebClient client = new WebClient())
{
    //Set the encoding to UTF8
    client.Encoding = System.Text.Encoding.UTF8;

    //Add the subscription key header
    client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
    client.Headers.Add("Content-Type", "application/json");
    responseString = client.UploadString(builder.Uri, postBody);
}
/*
得到的响应格式是JSON，如下：

{ "Answer": "Sample response", "Score": "0" }

然后那个response.cs是处理响应的

*/