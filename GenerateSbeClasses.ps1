
$jar = "C:\Users\mrhoy\.nuget\packages\sbe-tool\1.13.1\tools\sbe-tool-all.jar";
$OUTPUTDIR = "$PSScriptRoot\A19.StateMachine\PSharpBase\Distributed\Messages";
$SCHE = "$PSScriptRoot\A19.StateMachine\PSharpBase\Distributed\Messages\messages.xml";
java -jar $jar $SCHE #"-Dsbe.output.dir=$OUTPUTDIR" '-Dsbe.generate.ir="false"' '-Dsbe.target.language="uk.co.real_logic.sbe.generation.csharp.CSharp"' $SCHE