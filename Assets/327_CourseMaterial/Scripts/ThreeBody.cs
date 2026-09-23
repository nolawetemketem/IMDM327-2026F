// 3-G Starter Code
// Fall 2026. IMDM 327
// Instructor. Myungin Lee
using System.Globalization;
using System.Numerics;
using UnityEngine;

public class ThreeBody : MonoBehaviour
{
    public GameObject camera; 
    public int cameraDistance = 250;
    public int zRange = 20; // places the bodies in front of a camera near the origin looking along +Z. Try other positions too.
    public int speed = 10;
    public int scale = 100;
    public int numberOfSpheres = 100;
    public float radius = 30f;
    private const float G = 500 ; // Gravitational constant 
    
    BodyProperty[] bp;

    class BodyProperty  // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject body;
        public float mass;
        public UnityEngine.Vector3 velocity;
        public UnityEngine.Vector3 acceleration;
    }

    void Start()
    {
        camera.transform.position = new UnityEngine.Vector3(0, 0, -cameraDistance);
        camera.transform.eulerAngles = new UnityEngine.Vector3(0, 0, 0);

        // Allocate an array to store each body's properties.
        bp = new BodyProperty[numberOfSpheres];
        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSpheres; i++)
        {
            // Our gameobjects are created here:
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html

            // position is (x,y,z). In this case, I want to plot them on the circle with r
            // ******** Fill in this part ********
            float theta = ((float)i / (float)numberOfSpheres) * (2 * Mathf.PI);
            float randomMass = Random.Range(1f, 20f);
            float size = Mathf.Log(randomMass);
            float randomZ = Random.Range(-zRange, zRange);

            bp[i].body.transform.position = new UnityEngine.Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), randomZ);
            bp[i].velocity = UnityEngine.Vector3.zero; // Try different initial condition
            
         
            bp[i].body.transform.localScale = new UnityEngine.Vector3(size, size, size);
            bp[i].mass = randomMass;  // Simplified. Try different initial condition

            // + This is just pretty trails
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 3f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSpheres, 1f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f), // (color, normalized position)
                    new GradientColorKey(targetColor, 0.8f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f), // (alpha, normalized position) 
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;
        }
    }

     void FixedUpdate()
    {
        // Loop for N-body gravity
        // How should we design the loop?

        // 00. Initialize the acceleration for each body to zero at the start of each frame
       for (int i = 0; i < numberOfSpheres; i++)
            bp[i].acceleration = UnityEngine.Vector3.zero;
        
        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 0; i < numberOfSpheres; i++)
        {
            UnityEngine.Vector3 force = UnityEngine.Vector3.zero;
            UnityEngine.Vector3 r_i = bp[i].body.transform.position;
            // Something
            for ( int j = 0; j < numberOfSpheres; j++)
            {
                if (i != j) {
                    UnityEngine.Vector3 r_j = bp[j].body.transform.position, r_ij = r_j - r_i;
                    force = force + CalculateGravity(r_ij, bp[i].mass, bp[j].mass) / bp[i].mass;
                }
            }
            bp[i].acceleration = force;
        }

        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
        for (int i = 0; i < numberOfSpheres; i++)
        {
            bp[i].velocity = bp[i].velocity +  bp[i].acceleration * Time.fixedDeltaTime * speed;
            bp[i].body.transform.position = bp[i].body.transform.position + (bp[i].velocity / scale) * Time.fixedDeltaTime * speed;
        }
    }
    // Gravity Fuction to finish
    private UnityEngine.Vector3 CalculateGravity(UnityEngine.Vector3 distanceVector, float m1, float m2)
    {
        UnityEngine.Vector3 gravity = UnityEngine.Vector3.zero; // note this is also UnityEngine.Vector3
        // **** Fill in the function below. 
        gravity = G  * (m1 * m2 / distanceVector.sqrMagnitude) * distanceVector.normalized;
        return gravity;
    }
}