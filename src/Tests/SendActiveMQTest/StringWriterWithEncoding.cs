using System;
using System.IO;
using System.Text;

namespace SendActiveMQTest
{
    /// <summary>
    /// Overrides the base 'StringWriter' class to accept a different character encoding type.
    /// </summary>
    public class StringWriterWithEncoding : StringWriter
    {
        #region Properties

        /// <summary>
        /// Overrides the default encoding type (UTF-16).
        /// </summary>
        public override Encoding Encoding => _encoding ?? base.Encoding;

        #endregion

        #region Readonlys

        private readonly Encoding _encoding;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StringWriterWithEncoding() { }

        /// <summary>
        /// Constructor which accepts a character encoding type.
        /// </summary>
        /// <param name="encoding">The character encoding type</param>
        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        #endregion
    }
}
