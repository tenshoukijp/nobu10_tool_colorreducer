for %%A in (*.png) do (
    colorreducer %%A %%~nA.bmp -D2 -R1 
)
