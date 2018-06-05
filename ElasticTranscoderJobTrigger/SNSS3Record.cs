using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.LambdaFunctions.ElasticTranscoderJobTrigger
{
    public class SNSS3Record
    {
        #region Public Properties

        public string EventVersion { get; }

        public string EventSource { get; }

        public string AwsRegion { get; }

        public DateTime EventTime { get; }

        public string EventName { get; }

        public IDictionary<string, string> UserIdentity { get; }

        public IDictionary<string, string> RequestParameters { get; }

        public IDictionary<string, string> ResponseElements { get; }

        public S3Data S3 { get; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public SNSS3Record(
            string eventVersion,
            string eventSource,
            string awsRegion,
            DateTime eventTime,
            string eventName,
            IDictionary<string, string> userIdentity,
            IDictionary<string, string> requestParameters,
            IDictionary<string, string> responseElements,
            S3Data s3
            )
        {
            this.EventVersion = eventVersion;
            this.EventSource = eventSource;
            this.AwsRegion = awsRegion;
            this.EventTime = eventTime;
            this.EventName = eventName;
            this.UserIdentity = userIdentity;
            this.RequestParameters = requestParameters;
            this.ResponseElements = responseElements;
            this.S3 = s3;
        }

        #endregion

        #region Internal Classes

        public class S3Data
        {
            #region Public Properties

            public string S3SchemaVersion { get; }

            public string ConfigurationId { get; }

            public S3BucketData Bucket { get; }

            public S3ObjectData Object { get; }

            #endregion

            #region Constructors

            [JsonConstructor]
            public S3Data(
                string s3SchemaVersion,
                string configurationId,
                S3BucketData bucket,
                S3ObjectData @object
            )
            {
                this.S3SchemaVersion = s3SchemaVersion;
                this.ConfigurationId = configurationId;
                this.Bucket = bucket;
                this.Object = @object;
            }

            #endregion

            #region Internal Classes

            public class S3BucketData
            {
                #region Public Properties

                public string Name { get; }

                public IDictionary<string, string> OwnerIdentity { get; }

                public string Arn { get; }

                #endregion

                #region Constructors

                [JsonConstructor]
                public S3BucketData(
                    string name,
                    IDictionary<string, string> ownerIdentity,
                    string arn
                )
                {
                    this.Name = name;
                    this.OwnerIdentity = ownerIdentity;
                    this.Arn = arn;
                }

                #endregion
            }

            public class S3ObjectData
            {
                #region Public Properties

                public string Key { get; }

                public double Size { get; }

                public string ETag { get; }

                public string VersionId { get; }

                public string Sequencer { get; }

                #endregion

                #region Constructors

                [JsonConstructor]
                public S3ObjectData(
                    string key,
                    double size,
                    string eTag,
                    string versionId,
                    string sequencer
                )
                {
                    this.Key = key;
                    this.Size = size;
                    this.ETag = eTag;
                    this.VersionId = versionId;
                    this.Sequencer = sequencer;
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
