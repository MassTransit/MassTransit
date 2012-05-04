using System;

namespace MassTransit.Transports
{
	class RetryPolicy : ConnectionPolicy
	{
		readonly ConnectionPolicyChain _policyChain;

		public RetryPolicy(ConnectionPolicyChain policyChain)
		{
			_policyChain = policyChain;
		}

		public void Execute(Action callback)
		{
			callback();
			_policyChain.Pop(this);
			_policyChain.Next(callback);
		}
	}
}
