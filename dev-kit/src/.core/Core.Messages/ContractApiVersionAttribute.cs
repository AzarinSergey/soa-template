using System;

namespace Core.Messages
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContractApiVersionAttribute : Attribute
    {
        private readonly string _postfix;

        /// <summary>
        /// Interface version required for <see cref="IDiscoverableHttpService"/>>
        /// </summary>
        /// <param name="major">Backward not compatible changes</param>
        /// <param name="minor">Backward compatible changes</param>
        /// <param name="postfix">1.1-{postfix}</param>
        public ContractApiVersionAttribute(int major, int minor, string postfix = null)
        {
            _postfix = postfix;
            Major = major.ToString();
            Minor = minor.ToString();
        }

        public string Major { get; }

        public string Minor { get; }

        public string Full => $"{Major}.{Minor}{(_postfix == null ? null : "-" + _postfix )}";
    }
}