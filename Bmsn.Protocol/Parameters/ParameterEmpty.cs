using System;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Reprensets a valueles parameter
    /// </summary>
    public class ParameterEmpty : Parameter<object>
	{
        #region Initialization

        /// <summary>
        /// Initialize a new instance of class
        /// </summary>
        public ParameterEmpty()
            : base()
		{ }

        /// <summary>
        /// Initialize a new instance of this class with the provided name
        /// </summary>
        /// <param name="name"></param>
		public ParameterEmpty(string name) : base(name)
		{
		}

        #endregion

        #region Protected Methods

        protected override void OnValueChanging(object value)
        {
            if (value == null)
            {
                throw new Exception("You can not set any value to this type of parameter.");
            }
            base.OnValueChanging(value);
        }

        #endregion
    }
}
