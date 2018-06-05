using Amazon.ElasticTranscoder;
using Amazon.ElasticTranscoder.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using BAMCIS.AWSLambda.Common;
using BAMCIS.AWSLambda.Common.Events.SNS;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using static Amazon.Lambda.SNSEvents.SNSEvent;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BAMCIS.LambdaFunctions.ElasticTranscoderJobTrigger
{
    public class LambdaEntrypoint
    {
        #region Private Fields

        /// <summary>
        /// Placeholder for the lambda function context so
        /// other methods can access it to write logs
        /// </summary>
        private ILambdaContext _Context;

        /// <summary>
        /// The Elastic Transcoder client
        /// </summary>
        private IAmazonElasticTranscoder _ETClient = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public LambdaEntrypoint()
        {
            this._ETClient = new AmazonElasticTranscoderClient();
        }

        #endregion

        #region Public Methods

        public async Task PutObject(SNSEvent snsEvent, ILambdaContext context)
        {
            this._Context = context;

            this._Context.LogInfo($"Received SNS event:\n{JsonConvert.SerializeObject(snsEvent)}");

            ReadPresetResponse Preset = await this._ETClient.ReadPresetAsync(new ReadPresetRequest() { Id = Environment.GetEnvironmentVariable("PresetId") });

            if (Preset != null && (int)Preset.HttpStatusCode >= 200 && (int)Preset.HttpStatusCode <= 299)
            {
                foreach (SNSRecord Record in snsEvent.Records)
                {
                    string Message = Record.Sns.Message;
                    this._Context.LogInfo($"Message:\n{Message}");
                    SNSS3RecordSet RecordSet = JsonConvert.DeserializeObject<SNSS3RecordSet>(Message);

                    foreach (SNSS3Record S3Record in RecordSet.Records)
                    {
                        string Key = S3Record.S3.Object.Key;

                        DateTime Now = DateTime.Now;
                        string Prefix = $"{Now.Year.ToString()}/{Now.Month.ToString("00")}";
                        string FileName = Path.GetFileNameWithoutExtension(Key);

                        this._Context.LogInfo($"Submitting job for {Key}.");

                        CreateJobRequest Request = new CreateJobRequest()
                        {
                            PipelineId = Environment.GetEnvironmentVariable("Pipeline"),
                            Input = new JobInput()
                            {
                                AspectRatio = "auto",
                                Key = Key,
                                Container = "auto",
                                FrameRate = "auto",
                                Interlaced = "auto",
                                Resolution = "auto"
                            },
                            Output = new CreateJobOutput()
                            {
                                PresetId = Preset.Preset.Id,
                                Rotate = "0",
                                ThumbnailPattern = $"{Prefix}/{FileName}_{{count}}",
                                Key = $"{Prefix}/{FileName}.{Preset.Preset.Container}"
                            }
                        };

                        try
                        {
                            CreateJobResponse Response = await this._ETClient.CreateJobAsync(Request);

                            if ((int)Response.HttpStatusCode >= 200 && (int)Response.HttpStatusCode <= 299)
                            {
                                this._Context.LogInfo($"Successfully submitted job for {Response.Job.Input.Key} with id {Response.Job.Id} and arn {Response.Job.Arn}.");
                            }
                            else
                            {
                                this._Context.LogError($"Failed to successfully submit job for {Response.Job.Input.Key} with status code: {(int)Response.HttpStatusCode}");
                            }
                        }
                        catch (Exception e)
                        {
                            this._Context.LogError($"Failed to transcode {Key}.", e);
                        }
                    }
                }
            }
            else
            {
                this._Context.LogError($"Failed to retrieve information about preset {Environment.GetEnvironmentVariable("PresetId")}.");
            }
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
