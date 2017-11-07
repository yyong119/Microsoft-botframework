using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace Bot_Application3.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            string ss = activity.Text;

            string response = Send_Request(ss);

            JObject obj = JObject.Parse(response);
            float scoreval = float.Parse((string)(obj["score"]));
            if (scoreval >=5.0)
            {
                string ansval = (string)(obj["answer"]);
                
                // return our reply to the user
                await context.PostAsync(ansval/* + $"\tscore:{scoreval}"*/);

                context.Wait(MessageReceivedAsync);
            }
            /*
            else if (scoreval >= 5.0)
            {
                string reqstring = "{\"que\":\"" + ss + "\"}";

                string echostring = Post("cc", reqstring);

                JObject echojson = JObject.Parse(echostring);

                // return our reply to the user
                await context.PostAsync((string)(echojson["ans"]));

                context.Wait(MessageReceivedAsync);
            }
            */
            else
            {
                await context.PostAsync("对不起，小微的姿势水平还不足以回答这个问题哦");

                context.Wait(MessageReceivedAsync);
            }
        }

        private string Send_Request(string questionString)  //this is qnamaker
        {
            string responseString = string.Empty;

            var query = questionString; //User Query
            var knowledgebaseId = "2b1f9313-3c41-479f-b5df-65757c376f1b"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "d1ef0fbb784145a6b89694bc1feeb431"; //Use subscription key assigned to you.

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
            return responseString;
        }

        public string Post(string Url, string jsonParas)    //this is LUIS
        {
            string strURL = Url;
            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/json";
            //设置参数，并进行URL编码  
            string paraUrlCoded = jsonParas;//System.Web.HttpUtility.UrlEncode(jsonParas);    
            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流  
            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
                Console.Write("连接服务器失败!");
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流

            //String strValue = "";//strValue为http响应所返回的字符流

            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();

            //Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();

            return postContent;//返回Json数据

        }
    }
}