// 3-body Starter Code
// Fall 2026. IMDM 327
// Instructor. Myungin Lee
using System.Collections.Generic;
using System.Numerics;
// using System.Runtime.Intrinsics.X86;
using UnityEngine;
using System.Linq;
using System.Data.Common;

public class Flock : MonoBehaviour
{
    public float seperationF = .01f , cohesionF = .008f, alignmentF = .015f;
    public int relaventNeighbors = 5;
    public float radius = 100f;
            
    private const float G = 500f; // Gravitational constant for this simulation, not the real-world value.
    BodyProperty[] bp;
    private int numberOfSphere = 100;
    class BodyProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject body;
        public float mass;
        public UnityEngine.Vector3 velocity;
        public UnityEngine.Vector3 acceleration;
    }


    void Start()
    {
        // Allocate an array to store each body's properties.
        bp = new BodyProperty[numberOfSphere];
        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html

            // position is (x,y,z). In this case, I want to plot them on the circle with r
            
            float theta = ((float)i / (float)numberOfSphere) * (2 * Mathf.PI);
            
            // Debug.Log("theta: " + theta + " i: " + i);

            // ******** Fill in this part ********
            bp[i].body.transform.position = new UnityEngine.Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta) , 90);
            // z = 180 places the bodies in front of a camera near the origin looking along +Z. Try other positions too.

            bp[i].velocity = new UnityEngine.Vector3(-.1f,.5f,.2f); // Try different initial condition
            bp[i].mass = 1; // Simplified. Try different initial condition


            // + This is just pretty trails
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSphere, 1f, 1f);
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

    void Update()
    {

        // Loop for N-body gravity
        // How should we design the loop?
        for (int entity = 0; entity < numberOfSphere; entity++)
        {
            UnityEngine.Vector3 seperation = UnityEngine.Vector3.zero; 
            UnityEngine.Vector3 cohesion = UnityEngine.Vector3.zero; 
            UnityEngine.Vector3 alignment = UnityEngine.Vector3.zero; 
            Dictionary<int,float> neighbors = new Dictionary<int,float>();

            for (int other = 0; other < numberOfSphere; other++)
            {
                if (entity != other)
                {
                    float otherDistance = 
                        UnityEngine.Vector3.Distance(
                            bp[entity].body.transform.position, 
                            bp[other].body.transform.position);
                    if (neighbors.Count() < relaventNeighbors)
                    {
                        neighbors.Add(other, otherDistance);
                    } else
                    {
                        foreach (int neighbor in neighbors.Keys.ToList())
                        {
                            neighbors.TryGetValue(neighbor,  out float neighborDistance);
                            if (neighborDistance > otherDistance)
                            {
                                neighbors.Remove(neighbor);
                                neighbors.Add(other, otherDistance);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (int neighbor in neighbors.Keys.ToList())
            {
                neighbors.TryGetValue(neighbor,  out float neighborDistance);
                
                seperation += (1 / neighborDistance) * (bp[entity].body.transform.position - bp[neighbor].body.transform.position);
                cohesion += neighborDistance * (bp[neighbor].body.transform.position - bp[entity].body.transform.position);
                alignment += bp[neighbor].velocity;  
                
                // bp[neighbor].velocity;
                // bp[neighbor].body.transform.position;

                // UnityEngine.Vector3 containment;
                // UnityEngine.Vector3 arrival;
                // UnityEngine.Vector3 neighborhood;
            }

            bp[entity].velocity = (
                (seperation * seperationF) + (cohesion * cohesionF) + (alignment * alignmentF)
                ) / relaventNeighbors;
        

            bp[entity].body.transform.position += bp[entity].velocity;
        }
    }

    // Gravity Fuction to finish
    private UnityEngine.Vector3 CalculateGravity(UnityEngine.Vector3 distanceVector, float m1, float m2)
    {
        UnityEngine.Vector3 gravity = UnityEngine.Vector3.zero; // note this is also UnityEngine.Vector3
                                        // **** Fill in the function below. 
                                        // gravity = ****;
        return gravity;
    }
}

