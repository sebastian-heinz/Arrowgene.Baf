namespace Arrowgene.Baf
{
    public class BafPrng
    {
        private uint _seed;
        
        /// <summary>
        /// ADDR: 0x50E536 RVA: 0x10E536
        /// </summary>
        public BafPrng(uint seed)
        {
            _seed = seed;
        }
        
        /// <summary>
        /// ADDR: 0x50E53A RVA: 0x10E53A
        /// </summary>
        public uint Next()
        {
            _seed = _seed * 0x343fd + 0x269EC3;
            return (_seed >> 0x10) & 0x7FFF;
        }
    }
}