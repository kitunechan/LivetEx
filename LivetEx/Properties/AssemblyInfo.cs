using System.Reflection;
using System.Runtime.InteropServices;

using System.Windows.Markup;
using System;
using System.Resources; 

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。

[assembly: AssemblyTitle( "LivetEx" )]
[assembly: AssemblyDescription( "Livet v1.3 から改変したライブラリです。" )]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "LivetEx" )]
[assembly: AssemblyCopyright( "Copyright © 2015 fox" )]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: CLSCompliant(true)]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。

[assembly: ComVisible(false)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です

[assembly: Guid( "A8AFF0A3-9150-4854-8B27-91727D871F5D" )]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.3.*")]

[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx")]
[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx.Commands")]
[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx.Messaging")]
[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx.Behaviors")]
[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx.Triggers" )]
[assembly: XmlnsDefinition("http://schemas.livet-Ex/", "LivetEx.Converters" )]

[assembly: AssemblyFileVersion( "1.3.3" )]
