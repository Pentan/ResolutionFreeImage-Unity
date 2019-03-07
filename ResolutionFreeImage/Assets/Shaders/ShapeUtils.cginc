#ifndef RFI_SHAPEUTILS_CGINC
#define RFI_SHAPEUTILS_CGINC

float edgeWeight(float t, float smoothw) {
    return smoothstep(-smoothw, smoothw, t);
}

float modifyFactorToSphere(float f) {
    // Half Sphere
    float x = 1.0 - saturate(f);
    return sqrt(1.0 - x * x);
}

float modifyFactorToParabola(float f) {
    // Parabola
    float x = 1.0 - saturate(f);
    return 1.0 - x * x;
}

float modifyFactorToPow(float f, float p) {
    // Pow
    float x = 1.0 - saturate(f);
    return 1.0 - pow(x, p);
}

float glslmod(float x, float y) {
    return x - y * floor(x / y);
}

float2 glslmod(float2 x, float2 y) {
    return x - y * floor(x / y);
}

#endif
