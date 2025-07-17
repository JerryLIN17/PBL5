# Sashimono Joinery Learning in VR

This project is a Unity-based Virtual Reality application designed to help users learn, explore, and practice the traditional Japanese joinery technique called *Sashimono*. It was developed as part of the PBL5 course, using Unity and targeting the Meta Quest 3s headset.

---

## Features

- **Interactive VR Learning Environment**  
  Users can rotate and observe Sashimono joints in 3D and view assembly animations to understand their structure.

- **Test Mode with Voxel-Based Modeling**  
  Users recreate the joints by "cutting" voxel blocks to match the template structure.

- **Level System with Increasing Difficulty**  
  3 levels of complexity. In Level 2, users must generalize from partial instruction and build the rest independently.

- **Blueprint Hint System**  
  A simplified blueprint is available as a timed hint during testing, but use is tracked and penalized in evaluation.

- **Performance Tracking**  
  The system records learning and testing duration, cutting accuracy, undo actions, and blueprint usage for each user.

---

## Controls (Meta Quest 3s)

- **Confirm/Select**: Left/Right trigger  
- **Move**: Left joystick  
- **Rotate view**: Right joystick  
- **Grab object**: Left grip button  
- **Rotate grabbed object**: Left joystick  
- **Cut voxel**: Right grip button  
- **Undo last action**: "A" button (right controller)

---

## Experiment Modes

### Teaching Mode
- View complete or partial Sashimono model
- Rotate and explore with controller
- Watch step-by-step assembly animation
- Time is recorded

### Testing Mode
- Build joint structure by removing cubes
- Use blueprint (optional, 10s display with penalty)
- Assembly result checked automatically
- Time and accuracy recorded

---

## Requirements

- Unity 2022.3 LTS or higher  
- XR Interaction Toolkit  
- Meta Quest 3s headset (tested)  
- OpenXR setup with Oculus plugin

---

## For User

To run the project:

1. Open the project folder in Unity.
2. Load the **`LoadScene.unity`** scene inside `Scenes/`.
3. Build the project for Oculus support.
4. Deploy to Meta Quest 3s via USB3.


---

## Author

- **Name**: LIN Jie  
- **Student ID**: 26002304804  
- **Course**: PBL5 - Virtual Reality Learning  
- **University**: Ritsumeikan University
- **Laboratory**: Visual Information Engineering Laboratory

---

## License

This project is for educational purposes only.

