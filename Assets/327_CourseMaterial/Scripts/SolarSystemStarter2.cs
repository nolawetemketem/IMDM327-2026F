// IMDM327 Material
// Use CSV or JSON to load data into the simulation. Both formats are supported, but they use different data types. 
// The CSV format uses a struct, while the JSON format uses a class. This script demonstrates how to load both formats and access their data.
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
public class SolarSystemStarter2 : MonoBehaviour
{
    // These components can be attached independently.
    public GameObject camera; 
    public int cameraDistance = 3000;
    public long distanceScale = 1000000000; // tune this — adjust based on your actual data units
    public long radiusScale = 500000; 
    public int speed = 1000000;
    DataCSV solarCSV;
    DataJSON solarJSON;
    const float G = 6.674e-11f ; // Gravitational constant
    PlanetProperty[] planetProperties;
    private int numberOfSpheres = 10;
    class PlanetProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject planet;
        public float mass;
        public float radius;
        public UnityEngine.Vector3 velocity;
        public UnityEngine.Vector3 acceleration;
        public UnityEngine.Vector3 actualPosition;
    }

    // CSV data and JSON data use their own data types.
    public BodyProperty[] solarBodiesCSV;
    public SolarBody[] solarBodiesJSON;

    // Both loader scripts finish reading their files in Awake().
    void Start()
    {
        camera.transform.position = new UnityEngine.Vector3(0, cameraDistance, 0);
        camera.transform.eulerAngles = new UnityEngine.Vector3(90, 0, 0);
  
        // CSV: use this block when a DataCSV component is attached.
        solarCSV = GetComponent<DataCSV>();
        if (solarCSV != null)
        {
            solarBodiesCSV = solarCSV.bp;
            Debug.Log("Loaded " + solarBodiesCSV.Length + " bodies from solar.csv.");
            Debug.Log("First body: mass = " + solarBodiesCSV[0].mass + ", distance = " + solarBodiesCSV[0].distance + ", initial_velocity = " + solarBodiesCSV[0].initial_velocity);
        }

        // JSON: use this block when a DataJSON component is attached.
        solarJSON = GetComponent<DataJSON>();
        if (solarJSON != null)
        {
            solarBodiesJSON = solarJSON.solarData.bodies;
            Debug.Log("Loaded " + solarBodiesJSON.Length + " bodies from solar.json.");
            Debug.Log("First body: " + solarBodiesJSON[0].name + ", mass: " + solarBodiesJSON[0].mass);
        }

        // GameObject array to hold the planets in the simulation.
        planetProperties = new PlanetProperty[numberOfSpheres];
        for (int i = 0; i < numberOfSpheres; i++)
        {
            // Our gameobjects are created here:
            planetProperties[i] = new PlanetProperty();
            planetProperties[i].planet = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
        }

        // Apply the loaded data to the simulation. This is where you would set up your bodies in the scene based on the loaded data.
        for (int i = 0; i < solarBodiesJSON.Length; i++)
        {
            // What is missing here? You need to set the initial position and velocity of each planet based on the loaded data.
            // ***WRITE YOUR CODE HERE***
            float theta = Random.Range(0, 2 * Mathf.PI);          
            float r = solarBodiesJSON[i].distance ;
            float init_vel = solarBodiesJSON[i].initial_velocity;  
            float size = Mathf.Clamp((float)(solarBodiesJSON[i].radius / radiusScale), 0, radiusScale/25000);
           
            planetProperties[i].planet.name = solarBodiesJSON[i].name;
            planetProperties[i].mass = solarBodiesJSON[i].mass; 
            planetProperties[i].radius = size;
            planetProperties[i].actualPosition = new UnityEngine.Vector3(r * Mathf.Cos(theta), 0, r * Mathf.Sin(theta));
            planetProperties[i].planet.transform.localScale = new UnityEngine.Vector3(size,size,size);
            planetProperties[i].velocity = new UnityEngine.Vector3(init_vel * Mathf.Cos(theta + Mathf.PI/2), 0, init_vel * Mathf.Sin(theta + Mathf.PI/2));
            // Debug.Log(solarBodiesJSON[i].name + ": " + solarBodiesJSON[i].name + " -- " +  planetProperties[i].velocity) ;
            planetProperties[i].planet.transform.position = new UnityEngine.Vector3((r / distanceScale) * Mathf.Cos(theta), 0, (r / distanceScale) * Mathf.Sin(theta));
        

            // + This is just pretty trails
            TrailRenderer trailRenderer =  planetProperties[i].planet.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 7f;  // Width of the trail at the start
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
       for (int i = 1; i < numberOfSpheres; i++)
            planetProperties[i].acceleration = UnityEngine.Vector3.zero;

        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 1; i < numberOfSpheres; i++)
        {
            UnityEngine.Vector3 force = UnityEngine.Vector3.zero;
            UnityEngine.Vector3 r_i = planetProperties[i].actualPosition;
            
            // Something
            for ( int j = 0; j < numberOfSpheres; j++)
            {
                if (i != j) {
                    UnityEngine.Vector3 r_j = planetProperties[j].actualPosition, r_ij = r_j - r_i;
                    force = force + (CalculateGravity(r_ij, planetProperties[i].mass, planetProperties[j].mass) / planetProperties[i].mass);
                }
            }
            planetProperties[i].acceleration = force;
        }

        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
       for (int i = 1; i < numberOfSpheres; i++)
        {

            planetProperties[i].velocity = planetProperties[i].velocity + planetProperties[i].acceleration * Time.fixedDeltaTime * speed;

            planetProperties[i].actualPosition = planetProperties[i].actualPosition + planetProperties[i].velocity * Time.fixedDeltaTime * speed;
            planetProperties[i].planet.transform.position = planetProperties[i].planet.transform.position  + (planetProperties[i].velocity / distanceScale) * Time.fixedDeltaTime * speed;
    
        }
    } 

    // Gravity Fuction to finish
    private UnityEngine.Vector3 CalculateGravity(UnityEngine.Vector3 distanceVector, float m1, float m2)
    {
        UnityEngine.Vector3 gravity = UnityEngine.Vector3.zero; // note this is also Vector3
        gravity = G * (m1 * m2 / distanceVector.sqrMagnitude) * distanceVector.normalized;
        return gravity;
    }
}