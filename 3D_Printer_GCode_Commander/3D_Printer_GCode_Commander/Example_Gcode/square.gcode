; Square outline: 50x50mm
G21         ; Set units to millimeters
G90         ; Absolute positioning
G1 Z5 F500  ; Lift the tool up
G1 X0 Y0 F1500  ; Go to origin
G1 Z0 F500  ; Lower the tool (start cutting)
G1 X50 Y0 F1000 ; Move to (50, 0)
G1 X50 Y50      ; Move to (50, 50)
G1 X0 Y50       ; Move to (0, 50)
G1 X0 Y0        ; Back to origin
G1 Z5           ; Lift tool
M30             ; End program
