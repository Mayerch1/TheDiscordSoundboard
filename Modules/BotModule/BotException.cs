using System;

namespace BotModule
{
    /// <summary>
    /// Class for custom bot exception
    /// </summary>
    [Serializable]
    internal class BotException : System.Exception
    {
        /// <summary>
        /// type enum
        /// </summary>
        public enum type { connection, file, others }

        /// <summary>
        /// connectionError enum
        /// </summary>
        public enum connectionError { NoServer, NoChannel, NoStream, Token, Unspecified }

        /// <summary>
        /// Type property
        /// </summary>
        public type Type { get; set; }

        /// <summary>
        /// connectionError property
        /// </summary>
        public connectionError ConnectionError { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="_type">enum for error type</param>
        /// <param name="_msg">custom msg, wich describes the error</param>
        /// <param name="_conType">enum for connection error (Unspecified if non)</param>
        public BotException(type _type, string _msg, connectionError _conType = connectionError.Unspecified) : base(_msg)
        {
            Type = _type;
            ConnectionError = _conType;
        }
    }
}