# Unity UI tools
This tool allow to help work with unity UI
## How to use
After installation, the ***UI Helper*** folder will appear in the top panel, which will contain the main functions of the library
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/2d4f8d0c-20b0-4741-a012-120eda2b6145)
## At first ***Copy component window***
There you can copy all componetns or values from selected game object or create the full copy of it(for full sopy use ctrl + q), to select which object you want to copy to, you need to select the game object in the scene hierarchy.
#### How is this different from regular copying?
Its simple, when copying inactive or child objects in the normal way, copying them causes errors and difficulties, these functions are designed to save you from them

![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/0377217e-7adc-4df9-8232-fdb66a8c8a18)
## ***Edit canvas window***
There you can select the settings to apply to all or selected canvases on scene, also it have automatic default scales
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/27873144-abfe-4d2d-b926-a3484eb61af4)
## ***Edit text window***
This window can apply TMP_Text settings to all scene or to selected objects you can choose font, text, and size
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/d895e967-5278-4646-b3a3-1d0477839e56)
## ***Gradient window***
- Create gradient for
  - UI Images
  - TMP_text
- Save the gradient image to folder
- Create random gradient
- Mirror UI Image
- Copy gradeint from selected in scene hierarchy UI Image
- Spawn gradiented image or text
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/ed2efece-c125-4a6d-a920-53f83e984ae1)
# Custom UI Elements
To spawn custom element right click on hierarchy

![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/3a0bceff-6281-4019-a21f-670063e102f1)
## Button
With this button you can simplify enable or disable objects, and create custom change color or scale on pointer enter
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/ce974eff-21ac-4fdd-81cf-6fb4ed0a31fe)
## Radio button (toggle)
Change value on click. You can set any sprite to Checked state or change the image color when checked.
Objects to switch take the same state as the button value.
To use bool value from code use 
```cs
_radioButtonField.IsOn;
```
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/e44b3a59-d20b-409f-a0d7-eca6ddbbfcad)
## Radio group
The group of radio buttons, use this to keep only one button on. You can indicate the container transform to automatically fill the buttons list.
Use the inspector buttons to add radio button to container, or clear the list and contaner.
![image](https://github.com/Alastor606/Unity-UI-Tools/assets/114815838/94cd5ec2-91c4-434b-9376-c977336651b7)
## Scroll view 
Its a default scroll view with prepared settings, just for comfort.
## ***Hot keys***
- ctrl + q : copy
- ctrl + w : horizontal mirror image
- ctrl + e : vertical mirror image
- shift + 1 : random horizontal gradient
- shift + 2 : random vertical gradient
(Gradient works on Images & TMP_Text)




