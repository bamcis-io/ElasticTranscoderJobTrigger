using Newtonsoft.Json;
using System.Collections.Generic;

namespace BAMCIS.LambdaFunctions.ElasticTranscoderJobTrigger
{
    public class SNSS3RecordSet
    {
        #region Public Properties

        public IEnumerable<SNSS3Record> Records { get; }

        #endregion

        #region Constructors

        [JsonConstructor]
        public SNSS3RecordSet(IEnumerable<SNSS3Record> records)
        {
            this.Records = records;
        }

        #endregion
    }
}
