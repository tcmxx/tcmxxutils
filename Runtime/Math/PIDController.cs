
namespace TCUtils
{
    public class PIDController
    {
        public float KD { get; set; }
        public float KP { get; set; }
        public float KI { get; set; }

        public float PreviousError { get; set; }
        public float IntegratedError { get; set; }


        public float Step(float target, float lastActualOutput, float deltaT)
        {
            float error = target - lastActualOutput;
            float p = KP * error;
            float d = KD * (error - PreviousError) / deltaT;
            PreviousError = error;
            IntegratedError = IntegratedError + error * deltaT;
            float i = KI * IntegratedError;
            return p + d + i;
        }
    }
}