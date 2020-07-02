using System;

namespace Microsoft.StreamProcessing
{
    /// <summary>
    /// A node in the query plan representing function application to all input rows.
    /// </summary>
    public sealed class FusePlanNode : UnaryPlanNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="pipe"></param>
        /// <param name="keyType"></param>
        /// <param name="payloadType"></param>
        /// <param name="inputPayloadType"></param>
        /// <param name="isGenerated"></param>
        /// <param name="errorMessages"></param>
        public FusePlanNode(PlanNode previous, IQueryObject pipe,
            Type keyType, Type payloadType, Type inputPayloadType,
            bool isGenerated = false, string errorMessages = null)
            : base(previous, pipe, keyType, payloadType, inputPayloadType, isGenerated, errorMessages)
        {
        }
    }
}