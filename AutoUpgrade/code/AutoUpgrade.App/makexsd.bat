@Echo off
del schema0.xsd
xsd bin/AutoUpgrade.Lib.dll /type:AutoUpgrade
del AutoUpgrade.xsd
ren schema0.xsd AutoUpgrade.xsd
