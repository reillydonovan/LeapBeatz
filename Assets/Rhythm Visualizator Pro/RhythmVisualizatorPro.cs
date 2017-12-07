// Created by Carlos Arturo Rodriguez Silva (Legend)
// Video: https://www.youtube.com/watch?v=LXYWPNltY0s
// Contact: carlosarturors@gmail.com

// Rhythm Visualizator PRO //

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class RhythmVisualizatorPro : MonoBehaviour {

	#region Variables

	public GameObject soundBarPrefabCenter;
	public GameObject soundBarPrefabDownside;
	public Transform soundBarsTransform;



	[Header ("Audio Settings")] [Tooltip ("Do you want to listen all incoming audios from other AudioSources?")]
	public bool listenAllSounds;
	public AudioSource audioSource;

	[Space (5)]

	[HideInInspector]
	public GameObject [] soundBars;

	[Header ("Sound Bars Options [Requires Restart]")]
	[Range (32, 256)]
	public int barsQuantity = 160;

	public enum ScaleFrom
	{
		Center,
		Downside}

	;

	public ScaleFrom scaleFrom = ScaleFrom.Downside;
	int scaleFromNum = 2;

	[Header ("Camera Control")] [Tooltip ("Deactivate to use your own camera")]
	public bool cameraControl = true;
	[Tooltip ("Rotating around camera")]
	public bool rotateCamera = true;

	public Transform center;

	[Range (-35, 35)] [Tooltip ("Camera rotating velocity, positive = right, negative = left")]
	public float velocity = 15f;
	[Range (0, 200f)]
	public float height = 40f;
	[Range (0, 500)]
	public float orbitDistance = 300f;

	[Range (1, 179)]
	public int fieldOfView = 60;

	[Header ("Visualization Control")]

	public bool ScaleByRhythm = false;


	[Range (10, 200f)] [Tooltip ("Visualization Length")]
	public float length = 65f;

	public bool UseDefaultCameraOnChange = true;

	public enum Visualizations
	{
		Line,
		Circle,
		ExpansibleCircle,
		Sphere}

	;

	[Tooltip ("Visualization Form")]
	public Visualizations visualization = Visualizations.Line;

	[Range (1f, 50f)]
	public float extraScaleVelocity = 50f;

	[Header ("Levels Control")]
	[Range (0.75f, 15f)] [Tooltip ("Sound Bars global scale")]
	public float globalScale = 3f;
	[Range (1, 15)] [Tooltip ("Sound Bars smooth velocity to return to 0")]
	public int smoothVelocity = 3;

	public enum Channels
	{
		n512,
		n1024,
		n2048,
		n4096,
		n8192}

	;

	[Tooltip ("Large value of channels represents more spectrum values, you will need increase the SoundBars amount to represent all these values. Recommended: 4096, 2048")]
	public Channels channels = Channels.n2048;
	[Tooltip ("FFTWindow to use, it is a type of filter. Rectangular = Very Low filter, BlackmanHarris = Very High filter. Recommended = Blackman")]
	public FFTWindow method = FFTWindow.Blackman;
	int channelValue = 2048;

	[Header ("Auto Rhythm Particles [Experimental]")]
	public ParticleSystem rhythmParticleSystem;

	public bool autoRhythmParticles = true;

	[Tooltip ("Rhythm Sensibility, highter values is equal to more particles. Recommended: 5")]
	[Range (0f, 100f)]
	public float rhythmSensibility = 5;

	// Rhythm Minimum Sensibility. This don't need to change, use Rhythm Sensibility instead. Recommended: 1.5
	const float minRhythmSensibility = 1.5f;

	[Tooltip ("Amount of Particles to Emit")]
	[Range (1, 150)]
	public int amountToEmit = 100;

	[Tooltip ("Rhythm Particles Interval Time (Recommended: 0.05 Seconds).")]
	[Range (0.01f, 1f)]
	public float rhythmParticlesMaxInterval = 0.05f;

	float remainingRhythmParticlesTime;
	bool rhythmSurpassed = false;

	[Header ("Bass Control")] // Channel 0 (LEFT)
	[Range (1f, 300f)]
	public float bassSensibility = 40f;
	[Range (0.5f, 2f)]
	public float bassHeight = 1.5f;
	[Range (1, 5)]
	public int bassHorizontalScale = 1;
	[Range (0, 256)] [Tooltip ("Bass Horizontal Off-set")]
	public int bassOffset = 0;

	[Header ("Treble Control")] // Channel 1 (RIGHT)
	[Range (1f, 300f)]
	public float trebleSensibility = 80f;
	[Range (0.5f, 2f)]
	public float trebleHeight = 1.35f;
	[Range (1, 5)]
	public int trebleHorizontalScale = 3;
	[Range (0, 256)] [Tooltip ("Treble Horizontal Off-set, don't decrease or you will get bass values")]
	public int trebleOffset = 67;

	[Header ("Appearance Control")]
	public bool soundBarsParticles = true;

	[Tooltip ("Particles Interval Time (Recommended: 0.005 Seconds). 0 = No interval")]
	[Range (0f, 0.1f)]
	public float particlesMaxInterval = 0.005f;

	float remainingParticlesTime;
	bool surpassed = false;

	[Range (0.1f, 2f)]
	public float minParticleSensibility = 1.5f;
	public bool changeColor = true;

	[Range (0.1f, 5f)]
	public float colorIntervalTime = 3f;

	[Range (0.1f, 5f)]
	public float colorLerpTime = 2f;

	public Color [] colors = new Color[4];

	int posColor;

	Color newColor;
	Vector3 prevLeftScale;
	Vector3 prevRightScale;

	Vector3 rightScale;
	Vector3 leftScale;

	float timeChange;

	int halfBarsValue;

	int visualizationNumber = 1;

	float newLeftScale;

	float newRightScale;

	float rhythmAverage;

	int lastVisualization = 1;

	#endregion

	#region Extra

	/// <summary>
	/// Emits particles if there are rhythm.
	/// </summary>
	public void EmitIfThereAreRhythm () {
		float [] spectrumLeftData;
		float [] spectrumRightData;

		#pragma warning disable 618
		spectrumLeftData = audioSource.GetSpectrumData (channelValue, 0, method);
		spectrumRightData = audioSource.GetSpectrumData (channelValue, 1, method);
		#pragma warning restore 618
	
		int count = 0;
		float spectrumSum = 0;

		// Using bass data only
		for (int i = 0; i < 40; i++) {
			spectrumSum += Mathf.Max (spectrumLeftData [i], spectrumRightData [i]);
			count++;
		}

		rhythmAverage = (spectrumSum / count) * rhythmSensibility;

		// If the spectrum value exceeds the minimum 
		if (rhythmAverage >= minRhythmSensibility) {
			rhythmSurpassed = true;
		}

		// Auto Rhythm Particles
		if (autoRhythmParticles) {
			if (rhythmSurpassed) {
				// Emit particles
				rhythmParticleSystem.Emit (amountToEmit);
			}
		}
	}

	#endregion

	#region Start

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake () {
		//	Application.targetFrameRate = -1;

		// Get actual visualization
		if (visualization == Visualizations.Line) {
			visualizationNumber = 1;
		} else if (visualization == Visualizations.Circle) {
			visualizationNumber = 2;
		} else if (visualization == Visualizations.ExpansibleCircle) {
			visualizationNumber = 3;
		} else if (visualization == Visualizations.Sphere) {
			visualizationNumber = 4;
		}

		lastVisualization = visualizationNumber;

		// Check the prefabs
		if (soundBarPrefabCenter != null) {
			halfBarsValue = barsQuantity / 2;

			CreateCubes ();

			int value = Random.Range (1, 100);
			if (value > 50) {
				velocity = 15;
			} else {
				velocity = -15;
			}

		} else {
			Debug.LogWarning ("Please assign Sound Bar Prefab to the script");
			enabled = false;
		}
	}

	/// <summary>
	/// Creates the cubes.
	/// </summary>
	void CreateCubes () {

		// Instantiate the required Bars Quantity
		if (scaleFrom == ScaleFrom.Center) {
			for (int i = 0; i < barsQuantity; i++) {
				var clone = Instantiate (soundBarPrefabCenter, transform.position, Quaternion.identity) as GameObject;
				clone.transform.SetParent (soundBarsTransform.transform);
//				clone.GetComponentInChildren<ParticleSystem> ().startSpeed = particlesVelocity;
			}

			scaleFromNum = 1;
		} else if (scaleFrom == ScaleFrom.Downside) {
			for (int i = 0; i < barsQuantity; i++) {
				var clone = Instantiate (soundBarPrefabDownside, transform.position, Quaternion.identity) as GameObject;
				clone.transform.SetParent (soundBarsTransform.transform);
//				clone.GetComponentInChildren<ParticleSystem> ().startSpeed = particlesVelocity;
			}

			scaleFromNum = 2;
		}
			

		// Assign all these bars to the script
		try {
			soundBars = GameObject.FindGameObjectsWithTag ("SoundBar");
		} catch {
			Debug.LogWarning ("Please add the tag SoundBar in the editor. Edit > Project Settings > Tags and Layers > Tags > and create it, then assign that tag to the SoundBarPrefabs");
			enabled = false;
			return;
		}
		if (soundBars.Length <= 0) {
			Debug.LogWarning ("Please add the tag SoundBar in the editor. Edit > Project Settings > Tags and Layers > Tags > and create it, then assign that tag to the SoundBarPrefabs");
			enabled = false;
			return;
		}

		UpdateVisualizations ();
	}

	/// <summary>
	/// Change to the next form. TRUE = Next, FALSE = PREVIOUS
	/// </summary>
	/// <param name="next">If set to <c>true</c> next.</param>
	public void NextForm (bool next) {
		if (next) {
			visualizationNumber++;
		} else {
			visualizationNumber--;
		}

		if (visualizationNumber > 4) {
			visualizationNumber = 1;
		} else if (visualizationNumber <= 0) {
			visualizationNumber = 4;
		}

		if (visualizationNumber == 1) {
			visualization = Visualizations.Line;
		} else if (visualizationNumber == 2) {
			visualization = Visualizations.Circle;
		} else if (visualizationNumber == 3) {
			visualization = Visualizations.ExpansibleCircle;
		} else if (visualizationNumber == 4) {
			visualization = Visualizations.Sphere;
		}

		UpdateVisualizations ();
	}

	/// <summary>
	/// Updates the channels of audio.
	/// </summary>
	void UpdateChannels () {
		if (channels == Channels.n512) {
			channelValue = 512;
		} else if (channels == Channels.n1024) {
			channelValue = 1024;
		} else if (channels == Channels.n2048) {
			channelValue = 2048;
		} else if (channels == Channels.n4096) {
			channelValue = 4096;
		} else if (channels == Channels.n8192) {
			channelValue = 8192;
		}
	}

	#endregion

	#region Camera

	/// <summary>
	/// Change to Camera Predefined Positions
	/// </summary>
	void CameraPosition () {
		if (visualization == Visualizations.Line) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.z -= 170f;
			Camera.main.transform.position = cameraPos;
			cameraPos.y += 5f + height;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (center);


		} else if (visualization == Visualizations.Circle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += ((1f + height) / 20f);
			cameraPos.z += 5f; 
			Camera.main.transform.position = cameraPos;

			Camera.main.transform.LookAt (soundBarsTransform.position);

		} else if (visualization == Visualizations.ExpansibleCircle) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.y += 55f;
			Camera.main.transform.position = cameraPos;
			Camera.main.transform.LookAt (soundBarsTransform.position);
		

		} else if (visualization == Visualizations.Sphere) {
			Camera.main.fieldOfView = fieldOfView;
			var cameraPos = transform.position;
			cameraPos.z -= 40f;
			cameraPos.y += 5f + height;

			Camera.main.transform.position = cameraPos;

			Camera.main.transform.LookAt (soundBarsTransform.position);
			Camera.main.transform.position = cameraPos;
		}
	}

	void SetVisualizationPredefinedValues () {
		if (visualization == Visualizations.Line) {
			height = 40;
			orbitDistance = 450;


		} else if (visualization == Visualizations.Circle) {
			height = 40;
			orbitDistance = 125;


		} else if (visualization == Visualizations.ExpansibleCircle) {
			height = 40;
			orbitDistance = 175;



		} else if (visualization == Visualizations.Sphere) {
			height = 15;
			orbitDistance = 220;

		}
	}

	/// <summary>
	/// Camera Rotating Around Movement.
	/// </summary>
	void CameraMovement () {
		Camera.main.transform.position = center.position + (Camera.main.transform.position - center.position).normalized * orbitDistance;


		Camera.main.transform.RotateAround (center.position, Vector3.up, -velocity * Time.deltaTime);
	}

	#endregion

	#region ColorLerp

	Color currentColor;

	/// <summary>
	/// Change SoundBars and Particles Color.
	/// </summary>
	void ChangeColor () {
		
		if (scaleFromNum == 1) { // From Center
			currentColor = soundBars [0].GetComponent<Renderer> ().material.color;
		} else if (scaleFromNum == 2) { // From Downside
			currentColor = soundBars [0].transform.Find ("Cube").GetComponent<Renderer> ().material.color;
		}

		newColor = Color.Lerp (currentColor, colors [posColor], Time.deltaTime / colorLerpTime);

		if (scaleFromNum == 1) { // From Center
			foreach (GameObject cube in soundBars) {
				cube.GetComponentInChildren<Renderer> ().material.color = newColor;
				#pragma warning disable 618
				cube.GetComponentInChildren<ParticleSystem> ().startColor = newColor;
				rhythmParticleSystem.startColor = newColor;
				#pragma warning restore 618
			}	
		} else if (scaleFromNum == 2) { // From Downside
			foreach (GameObject cube in soundBars) {
				cube.transform.Find ("Cube").GetComponentInChildren<Renderer> ().material.color = newColor;
				#pragma warning disable 618
				cube.GetComponentInChildren<ParticleSystem> ().startColor = newColor;
				rhythmParticleSystem.startColor = newColor;
				#pragma warning restore 618
			}
		}
	}

	/// <summary>
	/// Change SoundBars and Particles Color Helper.
	/// </summary>
	void NextColor () {

		timeChange = colorIntervalTime;
		changeColor = false;

		if (posColor < colors.Length - 1) {
			posColor++;
		} else {
			posColor = 0;
		}
		changeColor = true;
	}

	#endregion

	#region Visualizations

	/// <summary>
	/// Updates the visualizations.
	/// </summary>
	public void UpdateVisualizations () {
		
		// Visualizations

		if (visualization == Visualizations.Circle) {
			for (int i = 0; i < barsQuantity; i++) {
				float angle = i * Mathf.PI * 2f / barsQuantity;
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var rot = soundBars [i].transform.eulerAngles;
				rot.x = 0;
				soundBars [i].transform.localEulerAngles = rot;
			}

		} else if (visualization == Visualizations.Line) {
			for (int i = 0; i < barsQuantity; i++) {
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos.x -= length * 5;
				pos.x += (length / barsQuantity) * (i * 10);

				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.localEulerAngles = Vector3.zero;
			}
		} else if (visualization == Visualizations.ExpansibleCircle) {
			for (int i = 0; i < barsQuantity; i++) {
				float angle = i * Mathf.PI * 2f / barsQuantity;
				Vector3 pos = soundBarsTransform.transform.localPosition;
				pos -= new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * length;
				soundBars [i].transform.localPosition = pos;
				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var newRot = soundBars [i].transform.eulerAngles;
				newRot.x -= 90;

				soundBars [i].transform.eulerAngles = newRot;
			}

		} else if (visualization == Visualizations.Sphere) {

			var points = UniformPointsOnSphere (barsQuantity, length);

			for (var i = 0; i < barsQuantity; i++) {

				soundBars [i].transform.localPosition = points [i];

				soundBars [i].transform.LookAt (soundBarsTransform.position);

				var rot = soundBars [i].transform.eulerAngles;
				rot.x -= 90;

				soundBars [i].transform.eulerAngles = rot;
			}
		}
			
		UpdateChannels ();

		if (cameraControl) {
			
			if (visualizationNumber != lastVisualization) {
				lastVisualization = visualizationNumber;

				if (UseDefaultCameraOnChange) {
					SetVisualizationPredefinedValues ();
				}
			}

			CameraPosition ();

		}
	}

	/// <summary>
	/// Create a Sphere with the given verticles number.
	/// </summary>
	/// <returns>The points on sphere.</returns>
	/// <param name="verticlesNum">Verticles number.</param>
	/// <param name="scale">Scale.</param>
	Vector3[] UniformPointsOnSphere (float verticlesNum, float scale) {
		var points = new List<Vector3> ();
		var i = Mathf.PI * (3 - Mathf.Sqrt (5));
		var o = 2 / verticlesNum;
		for (var k = 0; k < verticlesNum; k++) {
			var y = k * o - 1 + (o / 2);
			var r = Mathf.Sqrt (1 - y * y);
			var phi = k * i;
			points.Add (new Vector3 (Mathf.Cos (phi) * r, y, Mathf.Sin (phi) * r) * scale);
		}
		return points.ToArray ();
	}

	#endregion

	#region BaseScript

	/// <summary>
	/// Updates every frame this instance.
	/// </summary>
	void LateUpdate () {

		// Get Spectrum Data from Both Channels of audio
		float [] spectrumLeftData;
		float [] spectrumRightData;


		if (listenAllSounds) {
			// Get Spectrum Data from Both Channels of audio
			#pragma warning disable 618
			spectrumLeftData = AudioListener.GetSpectrumData (channelValue, 0, method);
			spectrumRightData = AudioListener.GetSpectrumData (channelValue, 1, method);
			#pragma warning restore 618
		} else {

			if (audioSource == null) {
				Debug.LogWarning ("Please assign an AudioSource or Active Listen All Sounds");
				return;
			}

			// Get Spectrum Data from Both Channels of audio
			#pragma warning disable 618
			spectrumLeftData = audioSource.GetSpectrumData (channelValue, 0, method);
			spectrumRightData = audioSource.GetSpectrumData (channelValue, 1, method);
			#pragma warning restore 618
		}

		// Wait for Rhythm Particles Interval (for performance)
		if (remainingRhythmParticlesTime <= 0) {
			
			int count = 0;
			float spectrumSum = 0;

			// Using bass data only
			for (int i = 0; i < 40; i++) {
				spectrumSum += Mathf.Max (spectrumLeftData [i], spectrumRightData [i]);
				count++;
			}

			rhythmAverage = (spectrumSum / count) * rhythmSensibility;


			// If the spectrum value exceeds the minimum 
			if (rhythmAverage >= minRhythmSensibility) {
				rhythmSurpassed = true;
			}

			// Auto Rhythm Particles
			if (autoRhythmParticles) {
				if (rhythmSurpassed) {
					// Emit particles
					rhythmParticleSystem.Emit (amountToEmit);
				}
			}
		}
			

		// Scale SoundBars Normally
		if (!ScaleByRhythm) {

			// SoundBars for Left Channel and Right Channel
			for (int i = 0; i < halfBarsValue; i++) {

				// Apply Off-Sets to get the AudioSpectrum
				int spectrumLeft = i * bassHorizontalScale + bassOffset;
				int spectrumRight = i * trebleHorizontalScale + trebleOffset;

				// Get Actual Scale from SoundBar in "i" position
				prevLeftScale = soundBars [i].transform.localScale;
				prevRightScale = soundBars [i + halfBarsValue].transform.localScale;

				var spectrumLeftValue = spectrumLeftData [spectrumLeft] * bassSensibility;
				var spectrumRightValue = spectrumRightData [spectrumRight] * trebleSensibility;

				// Left Channel //

				// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
				if (spectrumLeftValue >= minParticleSensibility) {

					// Apply extra scale to that SoundBar using Lerp
					newLeftScale = Mathf.Lerp (prevLeftScale.y,
					                           spectrumLeftValue * bassHeight * globalScale,
					                           Time.deltaTime * extraScaleVelocity);

					// If the Particles are activated, emit a particle too
					if (soundBarsParticles) {
						if (remainingParticlesTime <= 0) {
							soundBars [i].GetComponentInChildren<ParticleSystem> ().Play ();

							surpassed = true;
						}
					}
				} else {
					newLeftScale = Mathf.Lerp (prevLeftScale.y, spectrumLeftValue * globalScale, Time.deltaTime * smoothVelocity);
				}

				// If the New Scale is greater than Previous Scale, set the New Value to Previous Scale
				if (newLeftScale > prevLeftScale.y) {
					prevLeftScale.y = newLeftScale;
					leftScale = prevLeftScale;
				} else { // Else, Lerp to 0.1 value
					leftScale = prevLeftScale;
					leftScale.y = Mathf.Lerp (prevLeftScale.y, 0.1f, Time.deltaTime * smoothVelocity);
				} 

				// Set new scale
				soundBars [i].transform.localScale = leftScale;

				// Fix minimum Y Scale
				if (soundBars [i].transform.localScale.y < 0.11f) {
					soundBars [i].transform.localScale = new Vector3 (1f, 0.11f, 1f);
				}

				// Right Channel //

				// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
				if (spectrumRightValue >= minParticleSensibility) {

					// Apply extra scale to that SoundBar using Lerp
					newRightScale = Mathf.Lerp (prevRightScale.y,
					                            spectrumRightValue * trebleHeight * globalScale,
					                            Time.deltaTime * extraScaleVelocity);

					// If the Particles are activated, emit a particle too
					if (soundBarsParticles) {
						if (remainingParticlesTime <= 0f) {

							soundBars [i + halfBarsValue].GetComponentInChildren<ParticleSystem> ().Play ();

							surpassed = true;
						}
					}
				} else {
					newRightScale = Mathf.Lerp (prevRightScale.y, spectrumRightValue * globalScale, Time.deltaTime * smoothVelocity);
				}

				// If the New Scale is greater than Previous Scale, set the New Value to Previous Scale
				if (newRightScale > prevRightScale.y) {
					prevRightScale.y = newRightScale;
					rightScale = prevRightScale;
				} else { // Else, Lerp to 0.1
					rightScale = prevRightScale;
					rightScale.y = Mathf.Lerp (prevRightScale.y, 0.1f, Time.deltaTime * smoothVelocity);
				}

				// Set new scale
				soundBars [i + halfBarsValue].transform.localScale = rightScale;

				// Fix minimum Y Scale
				if (soundBars [i + halfBarsValue].transform.localScale.y < 0.11f) {
					soundBars [i + halfBarsValue].transform.localScale = new Vector3 (1f, 0.11f, 1f);
				}
			}

		} else { // Scale All SoundBars by Rhythm

			for (int i = 0; i < barsQuantity; i++) {
				
				prevLeftScale = soundBars [i].transform.localScale;

				// If Minimum Particle Sensibility is exceeded (volume is clamped beetween 0.01 and 1 to avoid 0)
				if (rhythmSurpassed) {

					// Apply extra scale to that SoundBar using Lerp
					newLeftScale = Mathf.Lerp (prevLeftScale.y,
					                           rhythmAverage * bassHeight * globalScale,
					                           Time.deltaTime * extraScaleVelocity);

					// If the Particles are activated, emit a particle too
					if (soundBarsParticles) {
						if (remainingParticlesTime <= 0f) {
							soundBars [i].GetComponentInChildren<ParticleSystem> ().Play ();

							surpassed = true;
						}
					}

				} else { 	// Else, Lerp to the previous scale
					newLeftScale = Mathf.Lerp (prevLeftScale.y,
					                           rhythmAverage * globalScale,
					                           Time.deltaTime * extraScaleVelocity);
				}

				// If the New Scale is greater than Previous Scale, set the New Value to Previous Scale
				if (newLeftScale > prevLeftScale.y) {
					prevLeftScale.y = newLeftScale;
					rightScale = prevLeftScale;
				} else { // Else, Lerp to 0.1
					rightScale = prevLeftScale;
					rightScale.y = Mathf.Lerp (prevLeftScale.y, 0.1f, Time.deltaTime * smoothVelocity);
				}

				// Set new scale
				soundBars [i].transform.localScale = rightScale;

				// Fix minimum Y Scale
				if (soundBars [i].transform.localScale.y < 0.11f) {
					soundBars [i].transform.localScale = new Vector3 (1f, 0.11f, 1f);
				}
			}
			

		}

		// Particles Interval Reset
		if (soundBarsParticles) {
			if (surpassed) {
				surpassed = false;
				remainingParticlesTime = particlesMaxInterval;
			} else {
				remainingParticlesTime -= Time.deltaTime;
			}
		}
			
		// Rhythm Interval Reset
		if (rhythmSurpassed) {
			rhythmSurpassed = false;
			remainingRhythmParticlesTime = rhythmParticlesMaxInterval;
		} else {
			remainingRhythmParticlesTime -= Time.deltaTime;
		}
	
			
		// Change Colors
		if (changeColor) {
			timeChange -= Time.deltaTime;

			// When the counter are less than 0, change to the next Color
			if (timeChange < 0f) {
				NextColor ();
			}

			// Execute color lerping
			ChangeColor ();
		}

		// Execute Camera Control
		if (cameraControl) {
			if (rotateCamera) {
				CameraMovement ();
			}
		}
	}

	#endregion
}