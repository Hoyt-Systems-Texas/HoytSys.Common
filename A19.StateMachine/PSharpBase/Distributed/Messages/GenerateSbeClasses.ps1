
$jar = "C:\Users\mrhoy\.nuget\packages\sbe-tool\1.13.1\tools\sbe-tool-all.jar";
$OUTPUTDIR = "$PSScriptRoot\A19.StateMachine\PSharpBase\Distributed\Messages";
$SCHE = "messages.xml";
java '-Dsbe.generate.ir="false"' '-Dsbe.xinclude.aware=true' '-Dsbe.target.language="uk.co.real_logic.sbe.generation.csharp.CSharp"' '-Dsbe.target.namespace=A19.StateMachine.PSharpBase.Distributed.Messages' '-Dsbe.csharp.generate.namespace.dir=false' -jar $jar messages.xml