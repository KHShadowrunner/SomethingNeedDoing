--so this will go to your GC desk try a single DOL turn in, then try to run a promotion. then teleport back to FC Estate.
--this is meant to be used from where ever you are after you acquire the daily DOL item

--borrowed some code and ideas from the wonderful:  (make sure the _functions is in the snd folder)
--https://github.com/elijabesu/ffxiv-scripts/blob/main/snd/_functions.lua
loadfiyel = os.getenv("appdata").."\\XIVLauncher\\pluginConfigs\\SomethingNeedDoing\\_functions.lua"
functionsToLoad = loadfile(loadfiyel)
functionsToLoad()
DidWeLoadcorrectly()

yield("/wait 0.5")
--CharacterSafeWait()
yield("/echo Attempting to turn in DOL turnin item")
TeleportToGCTown()
ZoneTransition()
WalkToGC()

 yield("/echo movement stopped - time for GC turn ins")
--yield("<wait.15>")
--yield("/waitaddon SelectString <maxwait.120>")
 yield("/visland stop")
yield("/wait 1")
yield("/target Personnel Officer")
yield("/wait 1")
yield("/send NUMPAD0")
yield("/pcall SelectString true 0 <wait.1>")
yield("/send NUMPAD0")
yield("/wait 1")
yield("/send NUMPAD0")
yield("/wait 1")
yield("/pcall GrandCompanySupplyList true 0 1 2")
yield("/wait 1")
yield("/send NUMPAD0")
yield("/wait 1")
yield("/send NUMPAD0")
yield("/wait 1")
yield("/send ESCAPE <wait.1.5>")
yield("/send ESCAPE <wait.1.5>")
yield("/wait 3")

if GetMaelstromGCRank() < 5 then
	--try to promote
	yield("/wait 1")
	yield("/target Personnel Officer")
	yield("/wait 1")
	yield("/send NUMPAD0")
	yield("/wait 1")
	yield("/send NUMPAD2")
	yield("/wait 0.5")
	yield("/send NUMPAD0")
	yield("/wait 0.5")
	yield("/send NUMPAD0")
	yield("/send ESCAPE <wait.1.5>")
	yield("/send ESCAPE <wait.1.5>")
	yield("/wait 3")

	--wait for char condition 1
	while GetCharacterCondition(32) == true and GetCharacterCondition(35) == true do
		yield("/wait 1")
	end
	yield("/wait 2")
end

if GetMaelstromGCRank() < 5 then
	--try to promote
	yield("/wait 1")
	yield("/target Personnel Officer")
	yield("/wait 1")
	yield("/send NUMPAD0")
	yield("/wait 1")
	yield("/send NUMPAD2")
	yield("/wait 0.5")
	yield("/send NUMPAD0")
	yield("/wait 0.5")
	yield("/send NUMPAD0")
	yield("/send ESCAPE <wait.1.5>")
	yield("/send ESCAPE <wait.1.5>")
	yield("/wait 3")

	--wait for char condition 1
	while GetCharacterCondition(32) == true and GetCharacterCondition(35) == true do
		yield("/wait 1")
	end
	yield("/wait 2")
end

-- Teleport back to FC House
yield("/tp Estate Hall <wait.10>")
yield("/waitaddon _ActionBar <maxwait.600><wait.5>")
for i=1, 20 do
  yield("/send W")
end

yield("/send Right")
yield("/send Right")

yield("/target Entrance <wait.1>")
yield("/lockon on")
yield("/automove on <wait.2.5>")
yield("/automove off <wait.1.5>")