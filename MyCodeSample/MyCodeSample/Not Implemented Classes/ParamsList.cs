using System.Collections.Generic;

namespace MyCodeSample.ViewModels
{
    internal class ParamsList
    {
        public static List<string> DirectNames = new() { "R0", "R1", "R2", "R3" };
        public static List<string> NullNames = new() { "R0", "X0", "R1", "X)" };
        public static List<string> IndivNames = new() { "R0", "R1", "R2", "R3" };

        public static Regex paramName = new Regex(@"(?<name>(?>R|X|B)\d{1,2})");
        public static Regex correctFloatFormat = new(@"((?<![\w.,])[+-]?(?:\d+(?>\.|\,)\d+|\d+)(?:[eE][+-]?\d+)?(?![\w.,]))");



        public static void ParamGetSumHandler(this SectionViewModel sec, object s)
        {
            var param = (s as ParamGeneral);
            var len = Convert.ToDecimal(sec.Lenght);
            param.sumValue = Convert.ToSingle(Convert.ToDecimal(param.SpecValue) * len);
            param.SumToolTip = (param.Data is TypNumber numDat) ?
            string.Format(ESResources.TXT_RANGE,
            numDat.MinValue * len,
            numDat.MaxValue * len,
            numDat.Step * len) :
            null;
        }

        public static void ParamSetSpecHandler(this SectionViewModel sec, object s)
        {
            var param = (s as ParamGeneral);
            param.specValue = Convert.ToDecimal(param.sumValue) / Convert.ToDecimal(sec.Lenght);
        }
    }
}
