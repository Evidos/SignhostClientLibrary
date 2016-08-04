using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Signhost.APIClient.Rest.ErrorHandling
{
#if SERIALIZABLE
	[Serializable]
	public partial class InternalServerErrorException
	{
		protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
#endif
}
