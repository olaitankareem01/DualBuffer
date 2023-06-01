
namespace DualBuffer.Models.Enums
{
    public class MakeCallViewModel
    {

        public double Di { get; set; }
        public double SNR { get; set; }

        public double WRB { get; set; }
        public int N { get; set; }
        public int Nco { get; set; }
        public  double deltaXi { get; set; }
        public double Xc { get; set; }

        public CallType callType { get; set; }


    }
}