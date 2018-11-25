﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.0.0.0 (NJsonSchema v9.12.2.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

namespace Greenergy.Emissions.API
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.0.0 (NJsonSchema v9.12.2.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class EmissionsClient 
    {
        private string _baseUrl = "http://localhost:5000";
        private System.Net.Http.HttpClient _httpClient;
        private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;
    
        public EmissionsClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient; 
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() => 
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }
    
        public string BaseUrl 
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }
    
        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }
    
        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<EmissionDataDTO>> GetMostRecentEmissionsAsync()
        {
            return GetMostRecentEmissionsAsync(System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<EmissionDataDTO>> GetMostRecentEmissionsAsync(System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/Emissions/latest");
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(System.Collections.Generic.ICollection<EmissionDataDTO>); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.ICollection<EmissionDataDTO>>(responseData_, _settings.Value);
                                return result_; 
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(System.Collections.Generic.ICollection<EmissionDataDTO>);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileResponse> UpdateEmissionsAsync(System.Collections.Generic.IEnumerable<EmissionDataMongo> emissions)
        {
            return UpdateEmissionsAsync(emissions, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> UpdateEmissionsAsync(System.Collections.Generic.IEnumerable<EmissionDataMongo> emissions, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/Emissions");
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(emissions, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is System.Enum)
            {
                string name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value;
                        }
                    }
                }
            }
            else if (value is bool) {
                return System.Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            return System.Convert.ToString(value, cultureInfo);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.0.0 (NJsonSchema v9.12.2.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class PrognosisClient 
    {
        private string _baseUrl = "http://localhost:5000";
        private System.Net.Http.HttpClient _httpClient;
        private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;
    
        public PrognosisClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient; 
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() => 
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }
    
        public string BaseUrl 
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }
    
        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }
    
        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<FileResponse> UpdatePrognosesAsync(System.Collections.Generic.IEnumerable<PrognosisDataMongo> prognoses)
        {
            return UpdatePrognosesAsync(prognoses, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> UpdatePrognosesAsync(System.Collections.Generic.IEnumerable<PrognosisDataMongo> prognoses, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/Prognosis");
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(prognoses, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<ConsumptionInfoDTO> OptimalConsumptionTimeAsync(int? consumptionMinutes, string consumptionRegion, System.DateTimeOffset? startNoEarlierThan, System.DateTimeOffset? finishNoLaterThan)
        {
            return OptimalConsumptionTimeAsync(consumptionMinutes, consumptionRegion, startNoEarlierThan, finishNoLaterThan, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<ConsumptionInfoDTO> OptimalConsumptionTimeAsync(int? consumptionMinutes, string consumptionRegion, System.DateTimeOffset? startNoEarlierThan, System.DateTimeOffset? finishNoLaterThan, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/api/Prognosis/optimize?");
            if (consumptionMinutes != null) 
            {
                urlBuilder_.Append("consumptionMinutes=").Append(System.Uri.EscapeDataString(ConvertToString(consumptionMinutes, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (consumptionRegion != null) 
            {
                urlBuilder_.Append("consumptionRegion=").Append(System.Uri.EscapeDataString(ConvertToString(consumptionRegion, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (startNoEarlierThan != null) 
            {
                urlBuilder_.Append("startNoEarlierThan=").Append(System.Uri.EscapeDataString(startNoEarlierThan.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (finishNoLaterThan != null) 
            {
                urlBuilder_.Append("finishNoLaterThan=").Append(System.Uri.EscapeDataString(finishNoLaterThan.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ConsumptionInfoDTO); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ConsumptionInfoDTO>(responseData_, _settings.Value);
                                return result_; 
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(ConsumptionInfoDTO);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is System.Enum)
            {
                string name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value;
                        }
                    }
                }
            }
            else if (value is bool) {
                return System.Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            return System.Convert.ToString(value, cultureInfo);
        }
    }
    
    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.12.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class EmissionDataDTO 
    {
        [Newtonsoft.Json.JsonProperty("emission", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Emission { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timeStampUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset TimeStampUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("region", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Region { get; set; }
    
        [Newtonsoft.Json.JsonProperty("createdOn", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset CreatedOn { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static EmissionDataDTO FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<EmissionDataDTO>(data);
        }
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.12.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class EmissionDataMongo 
    {
        [Newtonsoft.Json.JsonProperty("internalId", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public ObjectId InternalId { get; set; } = new ObjectId();
    
        [Newtonsoft.Json.JsonProperty("emission", Required = Newtonsoft.Json.Required.Always)]
        public int Emission { get; set; }
    
        [Newtonsoft.Json.JsonProperty("timeStampUTC", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.DateTimeOffset TimeStampUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("region", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public string Region { get; set; }
    
        [Newtonsoft.Json.JsonProperty("createdOn", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset CreatedOn { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static EmissionDataMongo FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<EmissionDataMongo>(data);
        }
    
    }
    
    /// <summary>Represents an ObjectId (see also BsonObjectId).</summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.12.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ObjectId 
    {
        /// <summary>Gets the timestamp.</summary>
        [Newtonsoft.Json.JsonProperty("timestamp", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Timestamp { get; set; }
    
        /// <summary>Gets the machine.</summary>
        [Newtonsoft.Json.JsonProperty("machine", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Machine { get; set; }
    
        /// <summary>Gets the PID.</summary>
        [Newtonsoft.Json.JsonProperty("pid", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Pid { get; set; }
    
        /// <summary>Gets the increment.</summary>
        [Newtonsoft.Json.JsonProperty("increment", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int Increment { get; set; }
    
        /// <summary>Gets the creation time (derived from the timestamp).</summary>
        [Newtonsoft.Json.JsonProperty("creationTime", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset CreationTime { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static ObjectId FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ObjectId>(data);
        }
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.12.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PrognosisDataMongo : EmissionDataMongo
    {
        [Newtonsoft.Json.JsonProperty("updatedOn", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset UpdatedOn { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static PrognosisDataMongo FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PrognosisDataMongo>(data);
        }
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.12.2.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ConsumptionInfoDTO 
    {
        [Newtonsoft.Json.JsonProperty("firstEmissions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double FirstEmissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("optimalEmissions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double OptimalEmissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lastEmissions", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double LastEmissions { get; set; }
    
        [Newtonsoft.Json.JsonProperty("firstConsumptionStartUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset FirstConsumptionStartUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("optimalConsumptionStartUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset OptimalConsumptionStartUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lastConsumptionStartUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastConsumptionStartUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("consumptionMinutes", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int ConsumptionMinutes { get; set; }
    
        [Newtonsoft.Json.JsonProperty("consumptionRegion", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ConsumptionRegion { get; set; }
    
        [Newtonsoft.Json.JsonProperty("prognosisUpdateTimeUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset PrognosisUpdateTimeUTC { get; set; }
    
        [Newtonsoft.Json.JsonProperty("lastPrognosisTimeUTC", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.DateTimeOffset LastPrognosisTimeUTC { get; set; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        
        public static ConsumptionInfoDTO FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ConsumptionInfoDTO>(data);
        }
    
    }

    public partial class FileResponse : System.IDisposable
    {
        private System.IDisposable _client; 
        private System.IDisposable _response; 

        public int StatusCode { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public System.IO.Stream Stream { get; private set; }

        public bool IsPartial
        {
            get { return StatusCode == 206; }
        }

        public FileResponse(int statusCode, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.IO.Stream stream, System.IDisposable client, System.IDisposable response)
        {
            StatusCode = statusCode; 
            Headers = headers; 
            Stream = stream; 
            _client = client; 
            _response = response;
        }

        public void Dispose() 
        {
            if (Stream != null)
                Stream.Dispose();
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.0.0 (NJsonSchema v9.12.2.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class SwaggerException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public SwaggerException(string message, int statusCode, string response, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException) 
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
        {
            StatusCode = statusCode;
            Response = response; 
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.0.0.0 (NJsonSchema v9.12.2.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class SwaggerException<TResult> : SwaggerException
    {
        public TResult Result { get; private set; }

        public SwaggerException(string message, int statusCode, string response, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException) 
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}