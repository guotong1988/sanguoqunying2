using UnityEngine;
using System.Collections;

public class ZhongWen {
	
	private static ZhongWen instance;
	public static ZhongWen Instance {
		get {
			if (instance == null) {
				instance = new ZhongWen();
			}
			return instance;
		}
	}
	
	private string[][] kingName;
	private string[] cityName;
	private string[] generalName;
	private string[] magicName;
	private string[] equipmentName;
	private string[] armsName;
	private string[] formationName;
	private string[] jobsName;
	
	public string ren = "人";
	public string nian = "年";
	public string bingfu = "兵符";
	public string zhishu = "之书";
	public string jublzh = "具备了指挥";
	public string denengli = "的能力。";
	public string xuehuile = "学会了";
	public string xiexia = "卸下";
	public string wurenzhanling = "无人占领";
	
	public string dengji = "等级";
	public string wuli = "武力";
	public string zhili = "智力";
	public string ming = "命";
	public string wei = "为";
	public string taishou = "太守";
	
	public string xiyuan = "西元";
	public string yue = "月";
	public string niezhengzhongle = "内政时间到了....";
	
	public string qiantichuzhen = "全体出阵放弃";
	public string cheng = "城？";
	
	public string chengTarget = "城";
	public string budui = "部队";
	
	public string jun = "军";
	
	public string daozei = "盗贼";
	
	public string zhaoxiang_no = "没有抓到任何武将。";
	public string zhaoxiang_ask = "将军，请加入我方，共创和平大业。";
	public string zhaoxiang_guilai = "将军安全归来，请再随我一起出征吧！";
	public string zhaoxiang_buzai = "目前不在城中，无法登用。";
	public string[] zhaoxiang_wenda = new string[]{
		"多谢大人爱戴，我愿效犬马之劳！",
		"太好了....",
		"你别多费唇舌了，忠臣不事二主！！",
		"唉！真是可惜....",
		"奸贼！！我是不会加入你的！",
		"唉！完全失败了....",
		"我是不会加入你的！！",
		"唉！失败了....",
		"多谢大人爱护，但人各有志何必强求....",
		"嗯！请再考虑考虑....",
		"末将必定赴汤蹈火！",
		"嗯，很好....",
	};
	
	public string meiyouwujiang = "没有可以执行命令的武将。";
	public string sousuo_nothing = "什么都找不到....";
	public string sousuo_rencai = "我找到了一名人才，请大人会见他。";
	public string sousuo_rencai_ask = "将军，请加入我方，共创和平大业。";
	public string[] sousuo_rencai_wenda = new string[]{
		"多谢大人爱戴，我愿效犬马之劳！",
		"嗯！这真是我军之福哪....",
		"多谢大人爱戴，我只想乐于农耕锄，懒于应付\n俗务，请恕在下不能答应！",
		"唉！真是可惜....",
	};
	public string sousuo_rencai_xinku = "将军，辛苦了!";
	public string sousuo_rencai_xinku_da = "这是我应该做的....";
	public string sousuo_wupin = "找到了物品";
	
	public string chucheng = "城防御力增加";
	public string kaifa = "城人口增加";
	
	public string shengqian_no = "没有可以升迁的武将。";
	public string shengqian_feng = "现在封";
	public string shengqian_wei = "为";
	public string shengqian_answer = "多谢大人！我当誓死效忠大人！";
	
	public string jindu = "进度";
	public string chengshu = "城数";
	public string wujiang = "武将";
	public string shijian = "时间";
	public string[] shuzi = new string[] 
			{ "一", "二", "三", "四",
				"五", "六", "七", "八"};
	
	public string zaodao = "遭到";
	public string degongji = "的攻击！";
	
	public string quanjuntuji = "全军突击！";
	public string wojundasheng = "我军大胜！俘虏敌将";
	public string wojunshengli = "我军胜利！！";
	public string wozhanbai = "我战败被俘了....";
	public string henyihan = "很遗憾，我战败了....";
	public string wojundahuoquansheng = "我军大获全胜！！";
	public string womengbeifude = "我们被俘的武将救回来了!";
	public string womenfulule = "我们俘虏了敌方武将";
	public string wojunzhanbai = "我军战败....";
	public string fuludewujiang = "俘虏的武将逃回去了....";
	public string beifulule = "将军被敌军俘虏了！";
	public string womenfululejunzhu = "我们俘虏了敌方君主，但是被逃了....";
	public string zhandoujieshu = "战斗结束.......";
	public string qingkuangbumiao = "情况不妙全军撤退....";
	
	public string wojun = "我军";
	public string zaoyu = "遭遇";
	public string fashengzhandou = "，发生战斗！";
	public string gongji = "攻击";
	
	public string dengjishengwei = "等级升为";
	public string ji = "级";
	public string zuidadaibing = "最大带兵数增加";
	public string tili = "体力";
	public string jili = "技力";
	public string zuidazhi = "最大值上升";
	
	public string houlie = "后列";
	public string qianlie = "前列";

	public string jieshuyu = "经过多年的征战，统一中国的梦想终于实现了！";
	public string yu = "于";
	public string tongyile = "统一了全中国。";
	public string henyihantongyi = "很遗憾，统一中国的梦想无法实现了....";

	public string daohao = "，";
	public string juhao = "。";
	public string tanhao = "！";
	public string shengluehao = "...";

	public string cheatSaveOver = "存档完成!";
	public string cheatCanNotSelect = "不能更改君主所在城市!";
	public string cheatSelectCity = "请先选择城市!";
	public string cheatSelectGeneral = "请先选择武将!";
	public string cheatFailChangeCity = "不能更改城市!";
	public string cheatFailChangeKing = "不能更改君主!";

	public string speedUp = "加速";
	public string normalSpeed = "正常";
	
	public string[] chuzhan1 = new string[]{
		"就让你看看我的厉害！",
		"反贼，还不速速下马受缚！",
		"不知死活的家伙，这不是你该来的地方！",
		"你的人头我是要定了，有什么遗言快交代吧！",
	};
	
	public string[] chuzhan2 = new string[]{
		"和我",
		"交手，你一定会后悔的！",
		"敢和我",
		"交手？来吧！",
		"谁敢和我",
		"交手！",
		"我乃",
		"，不怕死的人尽管过来吧！",
		"和我",
		"交手是你一生最大的错误！",
		"",
		"在此，谁敢与我决一死战！",
		"我乃",
		"是也！",
		"我是",
		"，来比试一下吧！",
	};
	
	public string[] chuzhan3 = new string[]{
		"别废话多说，一决胜负吧！",
		"只动嘴巴是不会赢的，来吧！",
	};
	
	public string[] chuzhan4 = new string[]{
		
	};
	
	public string[] chuzhan5 = new string[]{
		"反贼",
		"，准备受死吧",
		"反贼",
		"，早早受降可以免你一死！",
		"反贼",
		"，一决胜负吧！",
	};
	
	public string[] shengli1 = new string[] {
		"凭这两三招也敢出来献丑！",
		"你想赢我，还早一百年啊！",
		"就这点本事，也敢出来！",
		"早说过你会后悔的....",
		"真是不堪一击！",
		"要我手下留情，不嫌太晚了吗！",
		"凭你的身手，滚回家吃奶吧，哈哈哈....",
	};
	
	public string[] shengli2 = new string[] {
		"不要脸，每次都逃！",
		"哼，下次再宰了你！",
		"说到逃跑，你倒是满在行的....",
		"不知羞耻的家伙，有胆回头再与我较量！",
		"别跑那么快，小心跌下马来！",
	};
	
	public string[] shengli3 = new string[] {
		"",
		"鼠辈，竟然夹着尾巴逃了，哈哈哈....",
	};

    public string[] kingNames = {"曹操","刘备","孙坚","董卓","袁绍","袁术","公孙瓒","韩馥","孔融","乔瑁",
                                   "马腾","陶谦","孔岫","刘表","刘繇","严白虎","王朗","刘焉", "", 
                                   "何进","张角","丁原","龚景","乔玄","孙权","刘璋","张鲁","孟获","金旋","彻里吉"};

    public string GetKingName(int idx)
    {

        if (kingName == null)
        {
            kingName = new string[][] {
                new string[]{ "何进", "张角", "孙坚", "董卓", "丁原", "公孙瓒", "刘焉", "韩馥", "龚景", "乔玄",
                                        "陶谦", "孔岫", "刘繇", "严白虎" },
                new string[]{ "曹操","刘备","孙坚","董卓","袁绍","袁术","公孙瓒","韩馥","孔融","乔瑁",
                                        "马腾","陶谦","孔岫","刘表","刘繇","严白虎","王朗","刘焉"},
                new string[]{ "曹操", "刘备", "孙权", "袁绍", "马腾", "刘表", "刘璋", "张鲁" },
                new string[]{ "曹操", "刘备", "孙权", "马腾", "孟获", "刘璋", "张鲁", "金旋" },
                new string[]{ "曹操", "刘备", "孙权", "孟获", "彻里吉" }
            };
        }

        if (idx < 0 || idx >= kingName[Controller.MODSelect].Length)
			return "";

        return kingName[Controller.MODSelect][idx];
	}
	
	public string GetCityName(int idx) {
		if (cityName == null) {
			cityName = new string[] {
				"襄平", "北平", "代县", "晋阳", "南皮", "平原", "邺", "北海", "濮阳", "陈留",
				"洛阳", "弘农", "长安", "安定", "天水", "西凉", "下邳", "徐州", "许昌", "谯",
				"汝南", "宛", "新野", "襄阳", "上庸", "江夏", "江陵", "武陵", "长沙", "桂阳",
				"零陵", "寿春", "建业", "吴", "会稽", "庐江", "予章", "汉中", "下弁", "梓潼", 
				"成都", "永安", "江州", "建宁", "云南", "庐陵", "鄱阳", "夷州"
			};
		}
		
		if (idx < 0 || idx >= cityName.Length)
			return "";
		
		return cityName[idx];
	}
	
	public string GetGeneralName(int idx) {
		if (generalName == null) {
			generalName = new string[]{
				"丁奉", "于禁", "兀突骨", "公孙瓒", "卞喜", "太史慈", "孔岫", "孔融", "文钦", "文聘", 
				"文鸯", "文丑", "牛金", "王允", "王双", "司马炎", "司马昭", "司马师", "司马懿", "甘宁", 
				"田丰", "伊籍", "全琮", "忙牙长", "朱桓", "朵思大王", "吴兰", "吴懿", "吕布", "吕蒙", 
				"宋宪", "李典", "李恢", "李儒", "李严", "李傕", "步鹭", "沙摩诃", "车胄", "邢道荣", 
				"典韦", "周仓", "周泰", "周瑜", "孟达", "孟优", "孟获", "法正", "沮授", "金环三结", 
				"阿会喃", "姜维", "纪灵", "胡车儿", "凌统", "凌操", "夏侯惇", "夏侯渊", "夏侯霸", "孙坚", 
				"孙策", "孙权", "孙翊", "徐晃", "徐庶", "徐盛", "徐质", "祝融夫人", "荀攸", "荀彧", 
				"袁尚", "袁绍", "袁术", "袁熙", "袁谭", "郝昭", "马良", "马岱", "马超", "马腾", 
				"马谡", "高顺", "高览", "张任", "张松", "张虎", "张昭", "张苞", "张郃", "张飞", 
				"张鲁", "张辽", "张纮", "曹仁", "曹芳", "曹爽", "曹植", "曹彰", "曹操", "曹睿", 
				"许褚", "逢纪", "郭嘉", "郭图", "郭汜", "陈宫", "陈琳", "陈群", "陆抗", "陆逊", 
				"陶谦", "程昱", "程普", "华雄", "华歆", "黄忠", "黄祖", "黄盖", "黄权", "杨修", 
				"董允", "董卓", "董荼那", "贾充", "贾诩", "廖化", "满宠", "赵统", "赵云", "赵广", 
				"蒯良", "蒯越", "刘表", "刘焉", "刘备", "刘晔", "刘禅", "刘繇", "樊稠", "乐进", 
				"潘璋", "蒋济", "蒋琬", "蔡邕", "蔡瑁", "诸葛亮", "诸葛恪", "诸葛瑾", "鲁肃", "邓艾", 
				"邓忠", "邓芝", "卢植", "阎圃", "钟会", "韩当", "韩馥", "颜良", "魏延", "魏续", 
				"庞统", "庞德", "谯周", "关平", "关羽", "关索", "关兴", "严白虎", "严纲", "严舆", 
				"严颜", "公孙越", "王朗", "朱治", "辛评", "武安国", "皇甫嵩", "孙乾", "祖茂", "马玩", 
				"高沛", "张勋", "张济", "曹洪", "梁兴", "陈武", "陈登", "陈横", "乔玄", "乔瑁", 
				"关凤", "杨怀", "虞翻", "邹靖", "雷铜", "雷薄", "刘璋", "潘凤", "霍峻", "糜竺", 
				"糜芳", "韩嵩", "简雍", "阚泽", "曹丕", "貂蝉", "孙尚香", "何进", "张英", "丁原", 
				"张角", "张梁", "张宝", "程远志", "邓茂", "管亥", "赵弘", "韩忠", "龚都", "何仪", 
				"龚景", "曹真", "刘封", "董承", "董袭", "张闿", "张翼", "张嶷", "彻里吉", "臧霸", 
				"徐荣", "夏侯恩", "淳于琼", "曹休", "曹纯", "孙韶", "金旋", "公孙康", "向朗", "吕范", 
				"李异", "夏侯尚", "于吉", "左慈", "孙静", "桓范", "费祎", "轲比能", "董旻", "刘琦", 
				"刘琮", "蒋钦", "苏飞", "谭雄", "顾雍"
			};
		}
		
		if (idx < 0 || idx >= generalName.Length)
			return "";
		
		return generalName[idx];
	}
	
	public string GetGeneralName1(int idx) {
		
		string str = GetGeneralName(idx);
		if (str.Length == 2) {
			str = str[0] + "  " + str[1];
		}
		
		return str;
	}
	
	public string GetMagicName(int idx) {
		if (magicName == null) {
			magicName = new string[Informations.Instance.magicNum];
			
			magicName[0] = "命疗术";
			magicName[1] = "火箭";
			magicName[2] = "地雷";
			magicName[3] = "炎龙";
			magicName[4] = "雷击";
			magicName[5] = "落月弓";
			magicName[6] = "御飞刀";
			magicName[7] = "火箭烈";
			magicName[8] = "地雷震";
			magicName[9] = "炎龙无双";
			magicName[10] = "雷击闪";
			magicName[11] = "伏兵组阵";
			magicName[12] = "伏兵排阵";
			magicName[13] = "炎龙杀阵";
			magicName[14] = "黄龙天翔";
			magicName[15] = "五雷轰顶";
			magicName[16] = "火箭无袭";
			magicName[17] = "地雷震爆";
			magicName[18] = "天地无用";
			magicName[19] = "神剑";
			magicName[20] = "突石";
			magicName[21] = "冲车";
			magicName[22] = "八面火";
			magicName[23] = "鬼戟";
			magicName[24] = "落日弓";
			magicName[25] = "旋龙";
			magicName[26] = "落石";
			magicName[27] = "飞矢";
			magicName[28] = "半月斩";
			magicName[29] = "伏兵班阵";
			magicName[30] = "突石剑";
			magicName[31] = "四冲车";
			magicName[32] = "乱飞矢";
			magicName[33] = "神剑闪";
			magicName[34] = "八面火转";
			magicName[35] = "三日月斩";
			magicName[36] = "旋龙合壁";
			magicName[37] = "回天术";
			magicName[38] = "炬石轰";
			magicName[39] = "八门金锁";
			magicName[40] = "分身斩";
			magicName[41] = "炬石炼狱";
			magicName[42] = "神剑闪华";
			magicName[43] = "突剑四方";
			magicName[44] = "飞矢烈震";
			magicName[45] = "旋龙天舞";
			magicName[46] = "伏兵连阵";
			magicName[47] = "神鬼乱舞";
			magicName[48] = "日月轮斩";
			magicName[49] = "鬼哭神号";
		}
		if (idx < 0 || idx >= Informations.Instance.magicNum)
			return "";
		return magicName[idx];
	}
	
	public string GetEquipmentName(int idx) {
		if (equipmentName == null) {
			equipmentName = new string[Informations.Instance.equipmentNum];
			
			equipmentName[0] = "孙子兵法";
			equipmentName[1] = "孟德新书";
			equipmentName[2] = "兵书二十四篇";
			equipmentName[3] = "史记";
			equipmentName[4] = "春秋左传";
			equipmentName[5] = "遁甲天书";
			equipmentName[6] = "青囊书";
			equipmentName[7] = "太平要术";
			equipmentName[8] = "青龙偃月刀";
			equipmentName[9] = "方天画戟";
			equipmentName[10] = "丈八蛇矛";
			equipmentName[11] = "倚天剑";
			equipmentName[12] = "青虹剑";
			equipmentName[13] = "双铁戟";
			equipmentName[14] = "双刃斧";
			equipmentName[15] = "古锭刀";
			equipmentName[16] = "七星剑";
			equipmentName[17] = "双股剑";
			equipmentName[18] = "铁蒺黎骨朵";
			equipmentName[19] = "三尖刀";
			equipmentName[20] = "铁脊蛇矛";
			equipmentName[21] = "大斧";
			equipmentName[22] = "凤嘴刀";
			equipmentName[23] = "月牙戟";
			equipmentName[24] = "流星锤";
			equipmentName[25] = "短锥枪";
			equipmentName[26] = "眉尖刀";
			equipmentName[27] = "的卢马";
			equipmentName[28] = "赤兔马";
			equipmentName[29] = "爪黄飞电";
			equipmentName[30] = "大宛马";
		}
		if (idx < 0 || idx >= Informations.Instance.equipmentNum)
			return "";
		return equipmentName[idx];
	}
	
	public string GetArmsName(int code) {
		if (code == 0) return null;
		
		if (armsName == null) {
			armsName = new string[]{
					"朴刀兵",
					"长枪兵",
					"大刀兵",
					"弓箭兵",
					"鍊锤兵",
					"飞刀兵",
					"武斗兵",
					"蛮族兵",
					"铁鎚兵",
					"藤甲兵",
					"黄巾贼"};
		}
		
		int i = 0;
		while (code > 0) {
			if ((code & 0x1) == 1) {
				return armsName[i];
			}
			i++;
			code >>= 1;
		}
		
		return "";
	}
	
	public string GetFormationName(int code) {
		if (code == 0) return null;
		
		if (formationName == null) {
			formationName = new string[]{
				"方形阵法",
				"圆形阵法",
				"锥形阵法",
				"雁形阵法",
				"玄襄阵法",
				"鱼丽阵法",
				"鉤形阵法"};
		}

		int i = 0;
		while (code > 0) {
			if ((code & 0x1) == 1) {
				return formationName[i];
			}
			i++;
			code >>= 1;
		}
		
		return "";
	}
	
	public string GetJobsName(int idx) {
		if (jobsName == null) {
			jobsName = new string[] {
				"博士", "黄门令", "羽林监", "郎中", "太使令", "议郎", 
				"太使大夫", "御史中丞", "中常侍", "侍中", "尚书令", "长史", 
				"太傅", "太尉", "司徒", "司空", "大司马", "大司农", 
				"丞相",
				"典军校尉", "司隶校尉", "越骑校尉", "骁骑校尉", "裨将军", "偏将军", "牙门将军", "军师将军", "安国将军", "横江将军",
				"虎威将军", "伏波将军", "折冲将军", "鹰扬将军", "讨逆将军", "昭文将军", "荡寇将军", "扬武将军", "辅国将军", "镇军将军",
				"征虏将军", "龙骑将军", "前将军", "平北将军", "安南将军", "镇西将军", "征东将军", "卫将军", "车骑将军", "骠骑将军",
				"大将军"};
		}
		if (idx < 0 || idx >= Informations.Instance.jobsNum)
			return "";
		
		return jobsName[idx];
	}
}