for %%A in (*.png) do (
    colorreducer %%A %%~nA.bmp -R1 
)
