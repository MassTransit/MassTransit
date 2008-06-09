namespace SecurityMessages
{
    using System;

    [Serializable]
    public class PasswordUpdateComplete
    {
        private int _errorCode;

		private PasswordUpdateComplete()
		{
		}

    	public PasswordUpdateComplete(int errorCode)
        {
            _errorCode = errorCode;
        }


        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }
    }
}