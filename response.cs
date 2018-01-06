using Newtonsoft.Json; 

private class QnAMakerResult
{
    /// <summary>
    /// The top answer found in the QnA Service.
    /// </summary>
    [JsonProperty(PropertyName = "answer")]
    public string Answer { get; set; }

    /// <summary>
    /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
    /// </summary>
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set; }
}
//De-serialize the response
QnAMakerResult response;
try
{
    response = JsonConvert.DeserializeObject< QnAMakerResult >(responseString);
}
catch
{
    throw new Exception("Unable to deserialize QnA Maker response string.");
}