public class MathUtil
{
    public float LinearConverstion(float inputValue, float originalMin, float originalMax, float targetMin, float targetMax)
    {
        // Input value in the range of 0 to 1
        float result = (inputValue - originalMin) * (targetMax - targetMin) / (originalMax - originalMin) + targetMin; // Convert inputValue to the range of 10 to 20
        return result;
    }
}