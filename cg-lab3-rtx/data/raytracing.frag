#version 430
#define EPSILON 0.001
#define BIG 1000000.0
#define MAX_STACK_SIZE 32

const int DIFFUSE_REFLECTION = 1;
const int MIRROR_REFLECTION = 2;
const int REFRACTION = 3;

out vec4 FragColor;
in vec3 glPosition;

/*** DATA STRUCTURES ***/
struct SCamera 
{
    vec3 Position;
    vec3 View;
    vec3 Up;
    vec3 Side;
    vec2 Scale;
};
struct SRay 
{
    vec3 Origin;
    vec3 Direction;
};
struct SSphere
{
    vec3 Center;
    float Radius;
    int MaterialIdx;
};
struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialIdx;
};
struct SIntersection
{
    float Time;
    vec3 Point;
    vec3 Normal;
    vec3 Color;
    // ambient, diffuse and specular coeffs

    vec4 LightCoeffs;
    // 0 - non-reflection, 1 - mirror
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};
struct SLight
{
    vec3 Position;
};
struct SMaterial
{
    vec3 Color; //diffuse color
    vec4 LightCoeffs;       // ambient, diffuse and specular coeffs
    // 0 - non-reflection, 1 - mirror
    float ReflectionCoef;
    float RefractionCoef;
    int MaterialType;
};
struct STracingRay
{
    SRay ray;
    float contribution;
    int depth;
    bool isPrimary;
};
struct SCube {
    vec3 Center;
    float Size;
    int MaterialIdx;
};
struct STetrahedron {
    vec3 Center;
    float Size;
    int MaterialIdx;
};


/*** STACK IMPLEMENTATION ***/
STracingRay rayStack[MAX_STACK_SIZE];
int stackTop = -1;

bool pushRay(STracingRay newRay) {
    if (stackTop >= MAX_STACK_SIZE - 1) {
        return false;
    }
    stackTop++;
    rayStack[stackTop] = newRay;
    return true;
}

STracingRay popRay() {
    STracingRay res = rayStack[stackTop];
    stackTop--;
    return res;
}

bool isEmpty() {
    return stackTop < 0;
}


/*** INITIALIZATION ***/
SCamera initializeDefaultCamera() {
    //** CAMERA **//
    SCamera camera;
    camera.Position = vec3(0.0, 0.0, -8.0);
    camera.View = vec3(0.0, 0.0, 1.0);
    camera.Up = vec3(0.0, 1.0, 0.0);
    camera.Side = vec3(1.0, 0.0, 0.0);
    camera.Scale = vec2(1.0);
    return camera;
}

const int TOTAL_TRIANGLES = 12;
const int TOTAL_SPHERES = 2;
const int TOTAL_CUBES = 2;
const int TOTAL_TETRAHEDRONS = 1;
STriangle triangles[TOTAL_TRIANGLES];
SSphere spheres[TOTAL_SPHERES];
SCube cubes[TOTAL_CUBES];
STetrahedron tetrahedrons[TOTAL_TETRAHEDRONS];

void initializeDefaultScene()
{
    /** TRIANGLES **/
    /* left wall */
    triangles[0].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[0].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[0].v3 = vec3(-5.0, 5.0, -5.0);
    triangles[0].MaterialIdx = 2;
    triangles[1].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[1].v2 = vec3(-5.0, -5.0, 5.0);
    triangles[1].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[1].MaterialIdx = 2;

    /* back wall */
    triangles[2].v1 = vec3(-5.0, -5.0, 5.0);
    triangles[2].v2 = vec3(5.0, -5.0, 5.0);
    triangles[2].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[2].MaterialIdx = 1;
    triangles[3].v1 = vec3(5.0, 5.0, 5.0);
    triangles[3].v2 = vec3(-5.0, 5.0, 5.0);
    triangles[3].v3 = vec3(5.0, -5.0, 5.0);
    triangles[3].MaterialIdx = 1;

    /* right wall */
    triangles[4].v1 = vec3(5.0, 5.0, -5.0);
    triangles[4].v2 = vec3(5.0, 5.0, 5.0);
    triangles[4].v3 = vec3(5.0, -5.0, -5.0);
    triangles[4].MaterialIdx = 0;
    triangles[5].v1 = vec3(5.0, 5.0, 5.0);
    triangles[5].v2 = vec3(5.0, -5.0, 5.0);
    triangles[5].v3 = vec3(5.0, -5.0, -5.0);
    triangles[5].MaterialIdx = 0;

    /* bottom wall */
    triangles[6].v1 = vec3(5.0, -5.0, 5.0);
    triangles[6].v2 = vec3(-5.0, -5.0, 5.0);
    triangles[6].v3 = vec3(-5.0, -5.0, -5.0);
    triangles[6].MaterialIdx = 3;
    triangles[7].v1 = vec3(-5.0, -5.0, -5.0);
    triangles[7].v2 = vec3(5.0, -5.0, -5.0);
    triangles[7].v3 = vec3(5.0, -5.0, 5.0);
    triangles[7].MaterialIdx = 3;

    /* upper wall */
    triangles[8].v1 = vec3(5.0, 5.0, 5.0);
    triangles[8].v2 = vec3(-5.0, 5.0, -5.0);
    triangles[8].v3 = vec3(-5.0, 5.0, 5.0);
    triangles[8].MaterialIdx = 3;
    triangles[9].v1 = vec3(5.0, 5.0, 5.0);
    triangles[9].v2 = vec3(5.0, 5.0, -5.0);
    triangles[9].v3 = vec3(-5.0, 5.0, -5.0);
    triangles[9].MaterialIdx = 3;

    /* another wall */
    triangles[10].v1 = vec3(-5.0, 5.0, -5.0);
    triangles[10].v2 = vec3(5.0, -5.0, -5.0);
    triangles[10].v3 = vec3(-5.0, -5.0, -5.0);
    triangles[10].MaterialIdx = 3;
    triangles[11].v1 = vec3(-5.0, 5.0, -5.0);
    triangles[11].v2 = vec3(5.0, 5.0, -5.0);
    triangles[11].v3 = vec3(5.0, -5.0, -5.0);
    triangles[11].MaterialIdx = 3;


    /** SPHERES **/
    spheres[0].Center = vec3(-1.0, -1.0, -2.0);
    spheres[0].Radius = 2.0;
    spheres[0].MaterialIdx = 5;

    spheres[1].Center = vec3(2.0, 1.0, 2.0);
    spheres[1].Radius = 1.0;
    spheres[1].MaterialIdx = 4;

    /** CUBES **/
    cubes[0].Center = vec3(3.0, -3.0, 0.0);
    cubes[0].Size = 0.7;
    cubes[0].MaterialIdx = 2;

    cubes[1].Center = vec3(-1.0, -2.5, -4.0);
    cubes[1].Size = 0.5;
    cubes[1].MaterialIdx = 6;
    
    /** TETRAHEDRONS **/
    tetrahedrons[0].Center = vec3(0.0, 3.0, 0.0);
    tetrahedrons[0].Size = 1.2;
    tetrahedrons[0].MaterialIdx = 2;
}

uniform int maxRayDepth;

SLight light;
const int TOTAL_MATERIALS = 7;
SMaterial materials[TOTAL_MATERIALS];
// [0] - Green
// [1] - Blue
// [2] - Red
// [3] - White
// [4] - Mirror
// [5] - Glass
// [6] - Glass for cude

uniform vec3 uMaterialColor[TOTAL_MATERIALS];
uniform vec4 uMaterialLightCoeffs[TOTAL_MATERIALS];
uniform float uMaterialReflection[TOTAL_MATERIALS];
uniform float uMaterialRefraction[TOTAL_MATERIALS];
uniform int uMaterialType[TOTAL_MATERIALS];

SMaterial GetMaterial(int index) {
    SMaterial m;
    m.Color = uMaterialColor[index];
    m.LightCoeffs = uMaterialLightCoeffs[index];
    m.ReflectionCoef = uMaterialReflection[index];
    m.RefractionCoef = uMaterialRefraction[index];
    m.MaterialType = uMaterialType[index];
    return m;
}

void initializeDefaultLightMaterials()
{
    //** LIGHT **//
    // light.Position = vec3(0.0, 2.0, -4.0f);
    // light.Position = vec3(2.5, -0.5, -4.0f);
    light.Position = vec3(4.0f, 2.0f, -4.0f);
    

    /** MATERIALS **/
    vec4 lightCoefs = vec4(0.4, 0.9, 0.0, 512.0);

    // Green
    materials[0].Color = vec3(0.0, 1.0, 0.0);
    materials[0].LightCoeffs = vec4(lightCoefs);
    materials[0].ReflectionCoef = 0.5;
    materials[0].RefractionCoef = 1.0;
    materials[0].MaterialType = DIFFUSE_REFLECTION;

    // Blue
    materials[1].Color = vec3(0.0, 0.0, 1.0);
    materials[1].LightCoeffs = vec4(lightCoefs);
    materials[1].ReflectionCoef = 0.5;
    materials[1].RefractionCoef = 1.0;
    materials[1].MaterialType = DIFFUSE_REFLECTION;

    // Red
    materials[2].Color = vec3(1.0, 0.0, 0.0);
    materials[2].LightCoeffs = vec4(lightCoefs);
    materials[2].ReflectionCoef = 0.5;
    materials[2].RefractionCoef = 1.0;
    materials[2].MaterialType = DIFFUSE_REFLECTION;

    // White
    materials[3].Color = vec3(1.0, 1.0, 1.0);
    materials[3].LightCoeffs = vec4(0.55f, 0.9, 0.0, 512.0);
    materials[3].ReflectionCoef = 0.5;
    materials[3].RefractionCoef = 1.0;
    materials[3].MaterialType = DIFFUSE_REFLECTION;

    // Mirror
    materials[4].Color = vec3(0.9, 0.9, 1.0);
    materials[4].LightCoeffs = vec4(lightCoefs);
    materials[4].ReflectionCoef = 0.5;
    materials[4].RefractionCoef = 1.5;
    materials[4].MaterialType = MIRROR_REFLECTION;

    // Glass
    materials[5].Color = vec3(0.9, 0.9, 1.0);
    materials[5].LightCoeffs = vec4(lightCoefs);
    materials[5].ReflectionCoef = 0.1;
    materials[5].RefractionCoef = 2.5;
    materials[5].MaterialType = REFRACTION;

    
    // Glass for cude
    materials[6].Color = vec3(0.9, 0.9, 1.0);
    materials[6].LightCoeffs = vec4(0.4, 0.9, 0.8, 256.0);
    materials[6].ReflectionCoef = 0.1;
    materials[6].RefractionCoef = 1.3;
    materials[6].MaterialType = REFRACTION;
}


/*** RTX FUNCTIONS ***/
SRay GenerateRay(SCamera uCamera) {
    vec2 coords = glPosition.xy * uCamera.Scale;
    vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y;
    return SRay(uCamera.Position, normalize(direction));
}

bool IntersectSphere(SSphere sphere, SRay ray, float start, float final, out float time)
{
    ray.Origin -= sphere.Center;
    float A = dot(ray.Direction, ray.Direction);
    float B = dot(ray.Direction, ray.Origin);
    float C = dot(ray.Origin, ray.Origin) - sphere.Radius * sphere.Radius;
    float D = B * B - A * C;
    if (D > 0.0)
    {
        D = sqrt(D);
        //time = min ( max ( 0.0, ( -B - D ) / A ), ( -B + D ) / A );
        float t1 = (-B - D) / A;
        float t2 = (-B + D) / A;
        if (t1 < 0 && t2 < 0)
            return false;
        if (min(t1, t2) < 0)
        {
            time = max(t1, t2);
            return true;
        }
        time = min(t1, t2);
        return true;
    }
    return false;
}

bool IntersectTriangle(SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time)
{
    time = -1;
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;
    vec3 N = cross(A, B);

    float NdotRayDirection = dot(N, ray.Direction);
    if (abs(NdotRayDirection) < 0.001)
        return false;  

    float d = dot(N, v1);
    float t = -(dot(N, ray.Origin) - d) / NdotRayDirection;

    if (t < 0)
        return false;

    vec3 P = ray.Origin + t * ray.Direction;
    vec3 C;
    vec3 edge1 = v2 - v1;
    vec3 VP1 = P - v1;
    C = cross(edge1, VP1);
    if (dot(N, C) < 0)
        return false;

    vec3 edge2 = v3 - v2;
    vec3 VP2 = P - v2;
    C = cross(edge2, VP2);
    if (dot(N, C) < 0)
        return false;

    vec3 edge3 = v1 - v3;
    vec3 VP3 = P - v3;
    C = cross(edge3, VP3);
    if (dot(N, C) < 0)
        return false;

    time = t;
    return true;
}

float cubeIntersection(vec3 ro, vec3 rd, vec3 cubePos, float cubeSize) {
    vec3 cubeMin = cubePos - cubeSize;
    vec3 cubeMax = cubePos + cubeSize;
    
    vec3 invDir = 1.0 / rd;
    vec3 t0 = (cubeMin - ro) * invDir;
    vec3 t1 = (cubeMax - ro) * invDir;
    
    vec3 tmin = min(t0, t1);
    vec3 tmax = max(t0, t1);
    
    float tn = max(tmin.x, max(tmin.y, tmin.z));
    float tf = min(tmax.x, min(tmax.y, tmax.z));
    
    return (tn > tf || tf < 0.0) ? -1.0 : max(tn, 0.0);
}
vec3 cubeNormal(vec3 hitPoint, vec3 cubePos, vec3 rayDir) {
    vec3 localHit = hitPoint - cubePos;
    vec3 absHit = abs(localHit);
    vec3 normal;
    
    if(absHit.x > absHit.y && absHit.x > absHit.z) {
        normal = vec3(sign(localHit.x), 0.0, 0.0);
    } else if(absHit.y > absHit.z) {
        normal = vec3(0.0, sign(localHit.y), 0.0);
    } else {
        normal = vec3(0.0, 0.0, sign(localHit.z));
    }
    
    // Инверсия нормали если луч внутри куба
    float facing = dot(rayDir, normal);
    return (facing > 0.0) ? -normal : normal;
}

float tetrahedronIntersection(SRay ray, vec3 center, float size, out int faceIndex) {
    vec3 v0 = center + size * vec3( 1.0, -1.0, -1.0);
    vec3 v1 = center + size * vec3(-1.0, -1.0,  1.0);
    vec3 v2 = center + size * vec3(-1.0,  1.0, -1.0);
    vec3 v3 = center + size * vec3( 1.0,  1.0,  1.0);

    float minT = BIG;
    faceIndex = -1;
    float t;

    if (IntersectTriangle(ray, v0, v1, v2, t) && t < minT) { minT = t; faceIndex = 0; }
    if (IntersectTriangle(ray, v0, v2, v3, t) && t < minT) { minT = t; faceIndex = 1; }
    if (IntersectTriangle(ray, v0, v3, v1, t) && t < minT) { minT = t; faceIndex = 2; }
    if (IntersectTriangle(ray, v1, v2, v3, t) && t < minT) { minT = t; faceIndex = 3; }

    return (faceIndex != -1) ? minT : -1.0;
}
vec3 tetrahedronNormal(int faceIndex, vec3 center, float size) {
    vec3 v0 = center + size * vec3( 1.0, -1.0, -1.0);
    vec3 v1 = center + size * vec3(-1.0, -1.0,  1.0);
    vec3 v2 = center + size * vec3(-1.0,  1.0, -1.0);
    vec3 v3 = center + size * vec3( 1.0,  1.0,  1.0);

    vec3 normals[4];
    
    // Грань 0: v0-v1-v2
    normals[0] = normalize(cross(v1 - v0, v2 - v0));
    
    // Грань 1: v0-v2-v3
    normals[1] = normalize(cross(v2 - v0, v3 - v0));
    
    // Грань 2: v0-v3-v1
    normals[2] = normalize(cross(v3 - v0, v1 - v0));
    
    // Грань 3: v1-v2-v3
    normals[3] = normalize(cross(v2 - v1, v3 - v1));

    return normals[faceIndex];
}

bool Raytrace( SRay ray, float start, float final, inout SIntersection intersect, bool isPrimary)
{
    bool result = false;
    float test = start;
    intersect.Time = final;

    // calculate intersect with spheres
    for(int i = 0; i < TOTAL_SPHERES; i++)
    {
        SSphere sphere = spheres[i];
        if ( IntersectSphere (sphere, ray, start, final, test ) && test < intersect.Time )
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize ( intersect.Point - sphere.Center);

            if (dot(ray.Direction, intersect.Normal) > 0.0) {
                intersect.Normal = -intersect.Normal;
            }
            int matIdx = sphere.MaterialIdx;

            intersect.Color = GetMaterial(matIdx).Color;
            intersect.LightCoeffs = GetMaterial(matIdx).LightCoeffs;
            intersect.ReflectionCoef = GetMaterial(matIdx).ReflectionCoef;
            intersect.RefractionCoef = GetMaterial(matIdx).RefractionCoef;
            intersect.MaterialType = GetMaterial(matIdx).MaterialType;

            result = true;
        }
    }

    // calculate intersect with triangles
    for(int i = 0; i < TOTAL_TRIANGLES; i++)
    {
        // Пропускаем triangles[10] и triangles[11] для первичных лучей
        if (isPrimary && (i == 10 || i == 11)) continue;

        STriangle triangle = triangles[i];
        if (IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test) && test < intersect.Time)
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));

            // Для стекла нормаль должна быть обращена в правильную сторону
            if (dot(ray.Direction, intersect.Normal) > 0.0) {
                intersect.Normal = -intersect.Normal;
            }

            int matIdx = triangle.MaterialIdx;

            intersect.Color = GetMaterial(matIdx).Color;
            intersect.LightCoeffs = GetMaterial(matIdx).LightCoeffs;
            intersect.ReflectionCoef = GetMaterial(matIdx).ReflectionCoef;
            intersect.RefractionCoef = GetMaterial(matIdx).RefractionCoef;
            intersect.MaterialType = GetMaterial(matIdx).MaterialType;

            result = true;
        }
    }

    // calculate intersect with cubes
    for(int i = 0; i < TOTAL_CUBES; i++) {
        float test = cubeIntersection(ray.Origin, ray.Direction, 
                    cubes[i].Center, cubes[i].Size);
        if (test > start && test < intersect.Time) 
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = cubeNormal(intersect.Point, cubes[i].Center, ray.Direction);
            
            int matIdx = cubes[i].MaterialIdx;

            intersect.Color = GetMaterial(matIdx).Color;
            intersect.LightCoeffs = GetMaterial(matIdx).LightCoeffs;
            intersect.ReflectionCoef = GetMaterial(matIdx).ReflectionCoef;
            intersect.RefractionCoef = GetMaterial(matIdx).RefractionCoef;
            intersect.MaterialType = GetMaterial(matIdx).MaterialType;

            result = true;
        }
    }

    // calculate intersect with tetrahedrons
    for(int i = 0; i < TOTAL_TETRAHEDRONS; i++) {
        int faceIndex;
        float test = tetrahedronIntersection(ray, tetrahedrons[i].Center, 
                    tetrahedrons[i].Size, faceIndex);
        
        if(test > start && test < intersect.Time) {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = tetrahedronNormal(faceIndex, tetrahedrons[i].Center, tetrahedrons[i].Size);
            
            // Инверсия нормали если луч внутри тетраэдра
            if(dot(ray.Direction, intersect.Normal) > 0.0) {
                intersect.Normal = -intersect.Normal;
            }
            
            int matIdx = tetrahedrons[i].MaterialIdx;

            intersect.Color = GetMaterial(matIdx).Color;
            intersect.LightCoeffs = GetMaterial(matIdx).LightCoeffs;
            intersect.ReflectionCoef = GetMaterial(matIdx).ReflectionCoef;
            intersect.RefractionCoef = GetMaterial(matIdx).RefractionCoef;
            intersect.MaterialType = GetMaterial(matIdx).MaterialType;

            result = true;
        }
    }

    return result;
}

vec3 Unit;

vec3 Phong ( SIntersection intersect, SLight currLight, float shadow, SCamera uCamera)
{
    vec3 light = normalize( currLight.Position - intersect.Point );
    float diffuse = max(dot(light, intersect.Normal), 0.0);
    vec3 view = normalize(uCamera.Position - intersect.Point);
    vec3 reflected = reflect( -view, intersect.Normal );
    float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);

    return intersect.LightCoeffs.x * intersect.Color +
        intersect.LightCoeffs.y * diffuse * intersect.Color * shadow +
        intersect.LightCoeffs.z * specular * Unit;
}

float Shadow(SLight currLight, SIntersection intersect)
{
    float shadowing = 1.0;
    vec3 direction = normalize(currLight.Position - intersect.Point);
    float distanceLight = distance(currLight.Position, intersect.Point);
    SRay shadowRay = SRay(intersect.Point + direction * EPSILON, direction);

    SIntersection shadowIntersect;
    shadowIntersect.Time = BIG;

    if (Raytrace(shadowRay, 0, distanceLight, shadowIntersect, false))
    {
        shadowing = 0.0;
    }
    return shadowing;
}


/*** FRESNEL FUNCTION ***/
float fresnel(vec3 I, vec3 N, float n1, float n2) {
    float cosI = -dot(I, N);
    float eta = n1 / n2;
    float sinT2 = eta * eta * (1.0 - cosI * cosI);
    if (sinT2 > 1.0) return 1.0; // Total internal reflection
    float cosT = sqrt(1.0 - sinT2);
    float Rs = (n1 * cosI - n2 * cosT) / (n1 * cosI + n2 * cosT);
    float Rp = (n2 * cosI - n1 * cosT) / (n2 * cosI + n1 * cosT);
    return (Rs * Rs + Rp * Rp) / 2.0;
}


void main() {
    initializeDefaultScene();
    initializeDefaultLightMaterials();

    SCamera uCamera = initializeDefaultCamera();
    SRay primaryRay = GenerateRay(uCamera);

    vec3 resultColor = vec3(0, 0, 0);
    Unit = vec3(1.0);

    // Init stack
    STracingRay primaryTracingRay;
    primaryTracingRay.ray = primaryRay;
    primaryTracingRay.contribution = 1.0;
    primaryTracingRay.depth = 0;
    primaryTracingRay.isPrimary = true;
    
    pushRay(primaryTracingRay);


    while(!isEmpty()) {
        STracingRay trRay = popRay();
        SRay ray = trRay.ray;

        SIntersection intersect;
        intersect.Time = BIG;

        if (Raytrace(ray, 0.0, BIG, intersect, trRay.isPrimary)) {
            switch(intersect.MaterialType) {
                case DIFFUSE_REFLECTION: {
                    float shadowing = Shadow(light, intersect);
                    resultColor += trRay.contribution * Phong(intersect, light, shadowing, uCamera);
                    break;
                }
                case MIRROR_REFLECTION: {
                    if(intersect.ReflectionCoef < 1.0) {
                        float diffuseContribution = trRay.contribution * (1.0 - intersect.ReflectionCoef);
                        float shadowing = Shadow(light, intersect);
                        resultColor += diffuseContribution * Phong(intersect, light, shadowing, uCamera);
                    }
                    
                    if(trRay.depth < maxRayDepth) {
                        vec3 reflectDirection = reflect(ray.Direction, intersect.Normal);
                        float reflectionContribution = trRay.contribution * intersect.ReflectionCoef;
                        
                        STracingRay reflectRay;
                        reflectRay.ray.Origin = intersect.Point + reflectDirection * EPSILON;
                        reflectRay.ray.Direction = reflectDirection;
                        reflectRay.contribution = reflectionContribution;
                        reflectRay.depth = trRay.depth + 1;
                        reflectRay.isPrimary = false;

                        pushRay(reflectRay);
                    }
                    break;
                }
                case REFRACTION: {
                    if(trRay.depth < maxRayDepth) {
                        float n1 = 1.0; // Воздух
                        float n2 = intersect.RefractionCoef;
                        
                        bool isEntering = dot(ray.Direction, intersect.Normal) < 0.0;
                        vec3 normal = isEntering ? intersect.Normal : -intersect.Normal;
                        float eta = isEntering ? (n1 / n2) : (n2 / n1);

                        float fresnelCoef = fresnel(ray.Direction, normal, n1, n2);
                        
                        // Отражение
                        vec3 reflectDir = reflect(ray.Direction, normal);
                        STracingRay reflectRay = STracingRay(
                            SRay(intersect.Point + reflectDir * EPSILON, reflectDir),
                            trRay.contribution * fresnelCoef,
                            trRay.depth + 1,
                            false
                        );
                        pushRay(reflectRay);

                        // Преломление
                        vec3 refractDir = refract(ray.Direction, normal, eta);
                        if(length(refractDir) > 0.0) {
                            STracingRay refractRay = STracingRay(
                                SRay(intersect.Point + refractDir * EPSILON, refractDir),
                                trRay.contribution * (1.0 - fresnelCoef),
                                trRay.depth + 1,
                                false
                            );
                            pushRay(refractRay);
                        }
                    }
                    break;
                }
                default: {
                    resultColor = vec3(0.7, 0.7, 0.7);
                }
            }
            
        }
    }

    FragColor = vec4(resultColor, 1.0);
}
