#ifndef RFI_SHAPEUTILS_CGINC
#define RFI_SHAPEUTILS_CGINC

float edgeWeight(float t, float smoothw) {
    return smoothstep(-smoothw, smoothw, t);
}

#endif
