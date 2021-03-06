﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace KanTimeNotifier.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KanTimeNotifier.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似于 (Icon) 的 System.Drawing.Icon 类型的本地化资源。
        /// </summary>
        internal static System.Drawing.Icon _16 {
            get {
                object obj = ResourceManager.GetObject("_16", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   查找类似 
        ///「舰娘报时」
        ///==============================
        ///「舰娘报时」是一个舰队Collection的小程序，
        ///用于在整点时下载并播放指定舰娘的语音。
        ///
        ///程序内共记录了55个有报时语音的舰娘的信息，
        ///只需要在设置文件中[Setting]节点下设定ShipGirl选项
        ///为这55个舰娘中的一个就可以了。其他信息将由程序自动匹配。
        ///（请务必在设置 ShipGirl 的同时留空 ShipFileName!）
        ///若要设置程序内置以外的舰娘，则必须设定ShipFileName选项，
        ///填写为舰娘的kcs文件名，也就是立绘文件的文件名。
        ///（去向了解舰娘魔改的人求助吧）
        ///
        ///
        ///
        ///
        ///指定报时的舰娘
        ///-------------------------------
        ///程序内共记录了55个有报时语音的舰娘的信息，
        ///只需要在设置文件中 [Setting] 节点下将 ShipGirl 选项
        ///设定为舰娘的名字就可以了，
        ///其他信息将由程序自动匹配。
        ///
        ///ShipGirl 选项可以设为简体或日文繁体，
        ///只需要包含舰娘的名字就可以了。
        ///例如若要设置舰娘「千代田」的语音，
        ///那么 ShipGirl 就算是设成了「千代田@浮不起来」也是有效 [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string Help {
            get {
                return ResourceManager.GetString("Help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 ;
        ///; 应用程序「舰娘报时」的设置文档
        ///; 设置将在程序重新加载时生效
        ///; 详细说明请点击菜单项“阅读说明”
        ///;-------------------------------------------
        ///
        ///
        ///; 设置舰娘整点报时功能
        ///[Setting]
        ///ShipGirl=大和
        ///; 指定报时舰娘的名字
        ///ShipFileName=liqjxscjkogx
        ///; 或直接指定该舰娘的kcs文件名
        ///Download=1
        ///; 播放与下载模式
        ///; 0 - 直接播放URL
        ///; 1 - 下载到临时文件夹后播放
        ///; 2 - 下载到指定文件夹后播放
        ///Download2Folder=
        ///; 当Download=2时，保存mp3文件的位置
        ///Volume=80
        ///; 播放时声音的音量
        ///StartGamePath=
        ///; 当点击菜单项“启动游戏”时执行的地址
        ///
        ///
        ///
        ///; 设置通知区图标的信息提示
        ///[ToolTip]
        ///Title=#Name#
        ///; 提示标题。选项支持以下变量
        ///; #Name# - 舰娘名称
        ///; #JpName# - 日文名称
        ///; #Postfix# - 舰娘名称后缀
        ///; #ShipType# - 舰船类型
        ///; #CV# - 声优
        ///Icon [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string Setting {
            get {
                return ResourceManager.GetString("Setting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 &lt;?xml version=&quot;1.0&quot;?&gt;
        ///&lt;ArrayOfShip xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot; xmlns:xsd=&quot;http://www.w3.org/2001/XMLSchema&quot;&gt;
        ///  &lt;Ship&gt;
        ///    &lt;Name&gt;加贺&lt;/Name&gt;
        ///    &lt;JpName&gt;加賀&lt;/JpName&gt;
        ///    &lt;Postfix&gt;改&lt;/Postfix&gt;
        ///    &lt;FileName&gt;uuqdlbtrkmvk&lt;/FileName&gt;
        ///    &lt;ShipType&gt;正规空母&lt;/ShipType&gt;
        ///    &lt;CV&gt;井口裕香&lt;/CV&gt;
        ///  &lt;/Ship&gt;
        ///  &lt;Ship&gt;
        ///    &lt;Name&gt;金刚&lt;/Name&gt;
        ///    &lt;JpName&gt;金剛&lt;/JpName&gt;
        ///    &lt;Postfix&gt;改二&lt;/Postfix&gt;
        ///    &lt;FileName&gt;wugwvdccggcp&lt;/FileName&gt;
        ///    &lt;ShipType&gt;战舰&lt;/ShipType&gt;
        ///    &lt;CV&gt;東山奈央&lt;/CV&gt;
        ///  &lt;/Ship&gt;
        ///  &lt;Ship&gt;
        ///   [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string ShipList {
            get {
                return ResourceManager.GetString("ShipList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 #ship 大淀
        ///
        ///#25
        ///
        ///艦隊旗艦として特化設計された新鋭軽巡洋艦、大淀です。
        ///搭載予定の新型水偵の失敗や戦局の変化もあって、連合艦隊旗艦としてはあまり活躍できなかったの。
        ///でも、北号作戦や礼号作戦では活躍したのよ。私も前線で、頑張りますね。
        ///
        ///我是作为舰队旗舰而特化设计的新锐轻巡洋舰，大淀。
        ///由于搭载预定新型水上侦察机计划的失败和战局的变化，虽然身为联合舰队旗舰但是却没有什么功绩。
        ///不过，在北号作战与礼号作战力大放光彩了哟。我也会在前线努力的。	
        ///
        ///#1
        ///
        ///提督、軽巡大淀、戦列に加わりました。艦隊指揮、運営はどうぞお任せください。
        ///
        ///提督，轻巡大淀已加入队列，舰队的指挥和运营就交给我吧。
        ///
        ///#2
        ///
        ///提督、秋の気配を感じますね。
        ///
        ///提督，能感觉到秋天的气息呢。
        ///
        ///#4
        ///
        ///提督、そこは通信機ではありません。勝手に触られると、艦隊指揮に支障が出ます。
        ///
        ///提督，那里不是通讯器。请不要擅自乱碰，会对舰队的指挥有影响的。
        ///
        ///#3
        ///
        ///提督、その通信方法はどうなんでしょう。平文で良いかと思います。はい。
        ///
        ///提督，这个通讯方式感觉如何。我觉得平文（暗号文 [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string VoiceList {
            get {
                return ResourceManager.GetString("VoiceList", resourceCulture);
            }
        }
    }
}
