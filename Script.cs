static string tagMod = "[SSR]"; 
static string tagDebug = "DEBUG"; 
static float maxCollectedSolarPanel = 120;  
Dictionary<string,string> dataModule = new Dictionary<string,string> {};  
  
/**   
 * Apache License 2.0   
 * Written by lquattro (Scuriva Elquattro)  
 */   
public void Main(string argument) {  
  
    /**   
     * Variables texts  
     */   
    string textDebug = "   ---== Debug ==---";  
  
    // Solar Panels  
    List<IMySolarPanel> solarPanels = new List<IMySolarPanel>();     
    GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solarPanels);  
    // Rotor  
    List<IMyMotorStator> rotors = new List<IMyMotorStator>();      
    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors);  
  
    // Text Debug  
    List<IMyTextPanel> lcds = new List<IMyTextPanel>();       
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);  
 
    // Set Dictorary Map solar :  Module to Text 
    Dictionary<string,IMySolarPanel> solars = new Dictionary<string,IMySolarPanel> {}; 
    foreach ( IMySolarPanel solar in solarPanels) { 
        string solarName = solar.CustomName; 
        if ( !solarName.Contains(tagMod) ) continue; 

        string solarNameTag = solarName.Split(';')[1].ToUpper(); 
        solars[solarNameTag] = solar;
    } 
    
    foreach ( IMyMotorStator rotor in rotors ) { 
        string rotorName = rotor.CustomName;
        if ( !rotorName.Contains(tagMod) ) continue; 
 
        IMySolarPanel solar; 
        string solarNameTag = rotorName.Split(';')[1].ToUpper(); 
        if ( !solars.TryGetValue( solarNameTag, out solar ) ) continue; 
 
        // Test Solar panel power 
        float percentage = solar.MaxOutput * 1000 * 100 / maxCollectedSolarPanel; 
        textDebug += "\n Power kW: " + solar.MaxOutput * 1000; 
        textDebug += "\n Power %: " + percentage; 
        if ( percentage >= 80 ) { 
            rotor.SetValueFloat("Velocity", (float) 0); 
            continue; 
        }

        // Change Velocity 
        rotor.SetValueFloat("Velocity", (float) -0.10); 
        textDebug += "\n Angle: " + rotor.Angle; 
        textDebug += "\n Displacement: " + rotor.Displacement; 
        textDebug += "\n Torque: " + rotor.Torque; 
        textDebug += "\n Velocity: " + rotor.GetValueFloat("Velocity"); 
    }
  
    // Display Text Debug  
    foreach ( IMyTextPanel lcd in lcds ) { 
        if ( !lcd.CustomName.Contains(tagMod) && !lcd.CustomName.Contains(tagDebug) ) continue; 
 
        // Write test in displays  
        lcd.WritePublicText(textDebug, false);  
        // Show public text in displays   
        lcd.ShowPublicTextOnScreen();  
    }  
}   
