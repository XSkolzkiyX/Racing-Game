using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public VehicleData data;

    [Header("Camera")]
    [SerializeField] private CameraController mainCamera;

    [Header("Wheels")]
    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;

    [Header("Effects")]
    public ParticleSystem RLWParticleSystem;
    public ParticleSystem RRWParticleSystem;

    public TrailRenderer RLWTireSkid;
    public TrailRenderer RRWTireSkid;
    
    [Header("UI")]
    public TextMeshProUGUI carSpeedText;
    public TextMeshProUGUI scoreText;

    [HideInInspector]
    public float carSpeed;
    [HideInInspector]
    public bool isDrifting = false;
    [HideInInspector]
    public bool isTractionLocked;
    [HideInInspector]
    public int driftScore = 0;
    [HideInInspector]
    public bool canMove = true;

    Rigidbody carRigidbody; 
    float steeringAxis; 
    float throttleAxis; 
    float driftingAxis;
    float localVelocityZ;
    float localVelocityX;
    bool deceleratingCar;
    
    WheelFrictionCurve FLwheelFriction;
    float FLWextremumSlip;
    WheelFrictionCurve FRwheelFriction;
    float FRWextremumSlip;
    WheelFrictionCurve RLwheelFriction;
    float RLWextremumSlip;
    WheelFrictionCurve RRwheelFriction;
    float RRWextremumSlip;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = data.bodyMassCenter;

        #region WheelsFriction
        FLwheelFriction = new WheelFrictionCurve()
        {
            extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip,
            extremumValue = frontLeftCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue,
            stiffness = frontLeftCollider.sidewaysFriction.stiffness
        };
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction = new WheelFrictionCurve()
        {
            extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip,
            extremumValue = frontRightCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue,
            stiffness = frontRightCollider.sidewaysFriction.stiffness
        };
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction = new WheelFrictionCurve()
        {
            extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip,
            extremumValue = rearLeftCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue,
            stiffness = rearLeftCollider.sidewaysFriction.stiffness
        };
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction = new WheelFrictionCurve()
        {
            extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip,
            extremumValue = rearRightCollider.sidewaysFriction.extremumValue,
            asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip,
            asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue,
            stiffness = rearRightCollider.sidewaysFriction.stiffness
        };
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        #endregion
        if (!carSpeedText) carSpeedText = GameObject.Find("SpeedText").GetComponent<TextMeshProUGUI>();
        if (!scoreText) scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(CarSpeedUI), 0f, 0.1f);
    }

    private void Update()
    {
        if (!canMove) return;

        carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

        if (Input.GetKey(KeyCode.W))
        {
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            GoForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            GoReverse();
        }

        if (Input.GetKey(KeyCode.A)) TurnLeft();
        if (Input.GetKey(KeyCode.D)) TurnRight();

        if (Input.GetKey(KeyCode.Space))
        {
            CancelInvoke(nameof(DecelerateCar));
            deceleratingCar = false;
            Handbrake();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RecoverTraction();
        }
        if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)))
        {
            ThrottleOff();
        }
        if ((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar)
        {
            InvokeRepeating(nameof(DecelerateCar), 0f, 0.1f);
            deceleratingCar = true;
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f)
        {
            ResetSteeringAngle();
        }
        AnimateWheelMeshes();
    }

    public void CarSpeedUI()
    {
        float absoluteCarSpeed = Mathf.Abs(carSpeed);
        carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
    }

    public void TurnLeft()
    {
        steeringAxis -= Time.deltaTime * 10f * data.steeringSpeed;
        if (steeringAxis < -1f) steeringAxis = -1f;
        float steeringAngle = steeringAxis * data.maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, data.steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, data.steeringSpeed);
    }

    public void TurnRight()
    {
        steeringAxis += Time.deltaTime * 10f * data.steeringSpeed;
        if (steeringAxis > 1f) steeringAxis = 1f;
        float steeringAngle = steeringAxis * data.maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, data.steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, data.steeringSpeed);
    }

    public void ResetSteeringAngle()
    {
        if (steeringAxis < 0f)
        {
            steeringAxis += Time.deltaTime * 10f * data.steeringSpeed;
        }
        else if (steeringAxis > 0f)
        {
            steeringAxis -= Time.deltaTime * 10f * data.steeringSpeed;
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            steeringAxis = 0f;
        }
        float steeringAngle = steeringAxis * data.maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, data.steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, data.steeringSpeed);
    }

    private void AnimateWheelMeshes()
    {
        frontLeftCollider.GetWorldPose(out Vector3 FLWPosition, out Quaternion FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;
        frontRightCollider.GetWorldPose(out Vector3 FRWPosition, out Quaternion FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;
        rearLeftCollider.GetWorldPose(out Vector3 RLWPosition, out Quaternion RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;
        rearRightCollider.GetWorldPose(out Vector3 RRWPosition, out Quaternion RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
    }

    private void CheckDrift()
    {
        if (Mathf.Abs(localVelocityX) > 2.5f)
        {
            driftScore++;
            scoreText.text = driftScore.ToString();
            isDrifting = true;
        }
        else isDrifting = false;
        DriftCarPS();
        mainCamera.ChangeCameraAngle(localVelocityX);
    }

    public void GoForward()
    {
        CheckDrift();
        throttleAxis += Time.deltaTime * 3f;
        if (throttleAxis > 1f) throttleAxis = 1f;
        if (localVelocityZ < -1f) Brakes();
        else
        {
            if (Mathf.RoundToInt(carSpeed) < data.maxSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void GoReverse()
    {
        CheckDrift();
        throttleAxis -= Time.deltaTime * 3f;
        if (throttleAxis < -1f) throttleAxis = -1f;
        if (localVelocityZ > 1f) Brakes();
        else
        {
            if (Mathf.Abs(Mathf.RoundToInt(carSpeed)) < data.maxReverseSpeed)
            {
                frontLeftCollider.brakeTorque = 0;
                frontLeftCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                frontRightCollider.brakeTorque = 0;
                frontRightCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                rearLeftCollider.brakeTorque = 0;
                rearLeftCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
                rearRightCollider.brakeTorque = 0;
                rearRightCollider.motorTorque = (data.accelerationMultiplier * 50f) * throttleAxis;
            }
            else
            {
                frontLeftCollider.motorTorque = 0;
                frontRightCollider.motorTorque = 0;
                rearLeftCollider.motorTorque = 0;
                rearRightCollider.motorTorque = 0;
            }
        }
    }

    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }

    public void DecelerateCar()
    {
        CheckDrift();
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis = throttleAxis - (Time.deltaTime * 10f);
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis = throttleAxis + (Time.deltaTime * 10f);
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * data.decelerationMultiplier)));
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke(nameof(DecelerateCar));
        }
    }

    public void Brakes()
    {
        frontLeftCollider.brakeTorque = data.brakeForce;
        frontRightCollider.brakeTorque = data.brakeForce;
        rearLeftCollider.brakeTorque = data.brakeForce;
        rearRightCollider.brakeTorque = data.brakeForce;
    }

    public void Handbrake()
    {
        CancelInvoke(nameof(RecoverTraction));
        driftingAxis = driftingAxis + (Time.deltaTime);
        float secureStartingPoint = driftingAxis * FLWextremumSlip * data.handbrakeDriftMultiplier;

        if (secureStartingPoint < FLWextremumSlip) driftingAxis = FLWextremumSlip / (FLWextremumSlip * data.handbrakeDriftMultiplier);
        if (driftingAxis > 1f) driftingAxis = 1f;
        if (Mathf.Abs(localVelocityX) > 2.5f) isDrifting = true;
        else isDrifting = false;
        if (driftingAxis < 1f)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;
        }

        isTractionLocked = true;
        DriftCarPS();
    }

    public void DriftCarPS()
    {
        if (isDrifting)
        {
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
        }
        else if (!isDrifting)
        {
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
        }

        if ((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f)
        {
            RLWTireSkid.emitting = true;
            RRWTireSkid.emitting = true;
        }
        else
        {
            RLWTireSkid.emitting = false;
            RRWTireSkid.emitting = false;
        }
    }

    public void RecoverTraction()
    {
        isTractionLocked = false;
        driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
        if (driftingAxis < 0f) driftingAxis = 0f;

        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip * data.handbrakeDriftMultiplier * driftingAxis;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            Invoke(nameof(RecoverTraction), Time.deltaTime);

        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            FLwheelFriction.extremumSlip = FLWextremumSlip;
            frontLeftCollider.sidewaysFriction = FLwheelFriction;

            FRwheelFriction.extremumSlip = FRWextremumSlip;
            frontRightCollider.sidewaysFriction = FRwheelFriction;

            RLwheelFriction.extremumSlip = RLWextremumSlip;
            rearLeftCollider.sidewaysFriction = RLwheelFriction;

            RRwheelFriction.extremumSlip = RRWextremumSlip;
            rearRightCollider.sidewaysFriction = RRwheelFriction;

            driftingAxis = 0f;
        }
    }
}
