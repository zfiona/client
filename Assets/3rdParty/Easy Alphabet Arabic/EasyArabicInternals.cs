using System;
using System.Collections.Generic;

namespace EasyAlphabetArabic
{
	/// <summary>
	/// Internal class used to map Arabic only characters to the appropriate glyphs
	/// </summary>
	public static class EasyArabicInternals
	{
		private enum STATE
		{
			sIsolated,
			sInitial,
			sFinal,
			sMedial
		}

		private enum CASE
		{
			ccUnicode,
			ccIsolated,
			ccFinal,
			ccInitial,
			ccMedial
		}

		private enum _NumCase
		{
			cLatin,
			cArabic,
			cPersian
		}

		private static uint[,] arabicCharsArray = new uint[183, 5]
		{
			{
				9u,
				9u,
				9u,
				9u,
				9u
			},
			{
				32u,
				32u,
				32u,
				32u,
				32u
			},
			{
				33u,
				33u,
				33u,
				33u,
				33u
			},
			{
				34u,
				34u,
				34u,
				34u,
				34u
			},
			{
				10u,
				10u,
				10u,
				10u,
				10u
			},
			{
				1569u,
				65152u,
				65152u,
				0u,
				0u
			},
			{
				1570u,
				65153u,
				65154u,
				0u,
				0u
			},
			{
				1571u,
				65155u,
				65156u,
				0u,
				0u
			},
			{
				1573u,
				65159u,
				65160u,
				0u,
				0u
			},
			{
				1572u,
				65157u,
				65158u,
				0u,
				0u
			},
			{
				1574u,
				65161u,
				65162u,
				65163u,
				65164u
			},
			{
				1575u,
				65165u,
				65166u,
				0u,
				0u
			},
			{
				1576u,
				65167u,
				65168u,
				65169u,
				65170u
			},
			{
				1577u,
				65171u,
				65172u,
				0u,
				0u
			},
			{
				1578u,
				65173u,
				65174u,
				65175u,
				65176u
			},
			{
				1579u,
				65177u,
				65178u,
				65179u,
				65180u
			},
			{
				1580u,
				65181u,
				65182u,
				65183u,
				65184u
			},
			{
				1581u,
				65185u,
				65186u,
				65187u,
				65188u
			},
			{
				1582u,
				65189u,
				65190u,
				65191u,
				65192u
			},
			{
				1583u,
				65193u,
				65194u,
				0u,
				0u
			},
			{
				1584u,
				65195u,
				65196u,
				0u,
				0u
			},
			{
				1585u,
				65197u,
				65198u,
				0u,
				0u
			},
			{
				1586u,
				65199u,
				65200u,
				0u,
				0u
			},
			{
				1587u,
				65201u,
				65202u,
				65203u,
				65204u
			},
			{
				1588u,
				65205u,
				65206u,
				65207u,
				65208u
			},
			{
				1589u,
				65209u,
				65210u,
				65211u,
				65212u
			},
			{
				1590u,
				65213u,
				65214u,
				65215u,
				65216u
			},
			{
				1591u,
				65217u,
				65218u,
				65219u,
				65220u
			},
			{
				1592u,
				65221u,
				65222u,
				65223u,
				65224u
			},
			{
				1593u,
				65225u,
				65226u,
				65227u,
				65228u
			},
			{
				1594u,
				65229u,
				65230u,
				65231u,
				65232u
			},
			{
				1600u,
				1600u,
				1600u,
				1600u,
				1600u
			},
			{
				1601u,
				65233u,
				65234u,
				65235u,
				65236u
			},
			{
				1602u,
				65237u,
				65238u,
				65239u,
				65240u
			},
			{
				1603u,
				65241u,
				65242u,
				65243u,
				65244u
			},
			{
				1604u,
				65245u,
				65246u,
				65247u,
				65248u
			},
			{
				1605u,
				65249u,
				65250u,
				65251u,
				65252u
			},
			{
				1606u,
				65253u,
				65254u,
				65255u,
				65256u
			},
			{
				1607u,
				65257u,
				65258u,
				65259u,
				65260u
			},
			{
				1608u,
				65261u,
				65262u,
				0u,
				0u
			},
			{
				1609u,
				65263u,
				65264u,
				64488u,
				64489u
			},
			{
				1610u,
				65265u,
				65266u,
				65267u,
				65268u
			},
			{
				1611u,
				65136u,
				65136u,
				65136u,
				65137u
			},
			{
				1612u,
				65138u,
				65138u,
				65138u,
				65139u
			},
			{
				1613u,
				65140u,
				65140u,
				65140u,
				65141u
			},
			{
				1614u,
				65142u,
				65142u,
				65142u,
				65143u
			},
			{
				1615u,
				65144u,
				65144u,
				65144u,
				65145u
			},
			{
				1616u,
				65146u,
				65146u,
				65146u,
				65147u
			},
			{
				1617u,
				65148u,
				65148u,
				65148u,
				65149u
			},
			{
				1618u,
				65150u,
				65150u,
				65150u,
				65151u
			},
			{
				1649u,
				64336u,
				64337u,
				0u,
				0u
			},
			{
				1657u,
				64358u,
				64359u,
				64360u,
				64361u
			},
			{
				1658u,
				64350u,
				64351u,
				64352u,
				64353u
			},
			{
				1659u,
				64338u,
				64339u,
				64340u,
				64341u
			},
			{
				1660u,
				1660u,
				1660u,
				1660u,
				1660u
			},
			{
				1661u,
				0u,
				0u,
				0u,
				0u
			},
			{
				1662u,
				64342u,
				64343u,
				64344u,
				64345u
			},
			{
				1663u,
				64354u,
				64355u,
				64356u,
				64357u
			},
			{
				1664u,
				64346u,
				64347u,
				64348u,
				64349u
			},
			{
				1665u,
				1665u,
				1665u,
				1665u,
				1665u
			},
			{
				1667u,
				64374u,
				64375u,
				64376u,
				64377u
			},
			{
				1668u,
				64370u,
				64371u,
				64372u,
				64373u
			},
			{
				1669u,
				1669u,
				1669u,
				1669u,
				1669u
			},
			{
				1670u,
				64378u,
				64379u,
				64380u,
				64381u
			},
			{
				1671u,
				64382u,
				64383u,
				64384u,
				64385u
			},
			{
				1672u,
				64392u,
				64393u,
				0u,
				0u
			},
			{
				1673u,
				1673u,
				1673u,
				1673u,
				1673u
			},
			{
				1676u,
				64388u,
				64389u,
				0u,
				0u
			},
			{
				1677u,
				64386u,
				64387u,
				0u,
				0u
			},
			{
				1678u,
				64390u,
				64391u,
				0u,
				0u
			},
			{
				1681u,
				64396u,
				64397u,
				0u,
				0u
			},
			{
				1683u,
				1683u,
				1683u,
				1683u,
				1683u
			},
			{
				1686u,
				1686u,
				1686u,
				1686u,
				1686u
			},
			{
				1688u,
				64394u,
				64395u,
				0u,
				0u
			},
			{
				1690u,
				1690u,
				1690u,
				1690u,
				1690u
			},
			{
				1692u,
				1692u,
				1692u,
				1692u,
				1692u
			},
			{
				1700u,
				64362u,
				64363u,
				64364u,
				64365u
			},
			{
				1702u,
				64366u,
				64367u,
				64368u,
				64369u
			},
			{
				1705u,
				64398u,
				64399u,
				64400u,
				64401u
			},
			{
				1707u,
				1707u,
				1707u,
				1707u,
				1707u
			},
			{
				1709u,
				64467u,
				64468u,
				64469u,
				64470u
			},
			{
				1711u,
				64402u,
				64403u,
				64404u,
				64405u
			},
			{
				1713u,
				64410u,
				64411u,
				64412u,
				64413u
			},
			{
				1715u,
				64406u,
				64407u,
				64408u,
				64409u
			},
			{
				1722u,
				64414u,
				64415u,
				64414u,
				64415u
			},
			{
				1723u,
				64416u,
				64417u,
				64418u,
				64419u
			},
			{
				1724u,
				1724u,
				1724u,
				1724u,
				1724u
			},
			{
				1726u,
				64426u,
				64427u,
				64428u,
				64429u
			},
			{
				1728u,
				64420u,
				64421u,
				0u,
				0u
			},
			{
				1729u,
				64422u,
				64423u,
				64424u,
				64425u
			},
			{
				1733u,
				64480u,
				64481u,
				0u,
				0u
			},
			{
				1734u,
				64473u,
				64474u,
				0u,
				0u
			},
			{
				1735u,
				64471u,
				64472u,
				0u,
				0u
			},
			{
				1736u,
				64475u,
				64476u,
				0u,
				0u
			},
			{
				1737u,
				64482u,
				64483u,
				0u,
				0u
			},
			{
				1739u,
				64478u,
				64479u,
				0u,
				0u
			},
			{
				1740u,
				64508u,
				64509u,
				64510u,
				64511u
			},
			{
				1741u,
				1741u,
				1741u,
				1741u,
				1741u
			},
			{
				1744u,
				64484u,
				64485u,
				64486u,
				64487u
			},
			{
				1749u,
				1749u,
				1749u,
				1749u,
				1749u
			},
			{
				1746u,
				64430u,
				64431u,
				0u,
				0u
			},
			{
				1747u,
				64432u,
				64433u,
				64432u,
				64432u
			},
			{
				8205u,
				8205u,
				8205u,
				8205u,
				8205u
			},
			{
				1731u,
				1731u,
				1731u,
				1731u,
				1731u
			},
			{
				1611u,
				65136u,
				65136u,
				65136u,
				65137u
			},
			{
				1612u,
				65138u,
				65138u,
				65138u,
				65139u
			},
			{
				1613u,
				65140u,
				65140u,
				65140u,
				65141u
			},
			{
				1614u,
				65142u,
				65142u,
				65142u,
				65143u
			},
			{
				1615u,
				65144u,
				65144u,
				65144u,
				65145u
			},
			{
				1616u,
				65146u,
				65146u,
				65146u,
				65147u
			},
			{
				1617u,
				65148u,
				65148u,
				65148u,
				65149u
			},
			{
				1618u,
				65150u,
				65150u,
				65150u,
				65151u
			},
			{
				1632u,
				1632u,
				1632u,
				1632u,
				1632u
			},
			{
				1633u,
				1633u,
				1633u,
				1633u,
				1633u
			},
			{
				1634u,
				1634u,
				1634u,
				1634u,
				1634u
			},
			{
				1635u,
				1635u,
				1635u,
				1635u,
				1635u
			},
			{
				1636u,
				1636u,
				1636u,
				1636u,
				1636u
			},
			{
				1637u,
				1637u,
				1637u,
				1637u,
				1637u
			},
			{
				1638u,
				1638u,
				1638u,
				1638u,
				1638u
			},
			{
				1639u,
				1639u,
				1639u,
				1639u,
				1639u
			},
			{
				1640u,
				1640u,
				1640u,
				1640u,
				1640u
			},
			{
				1641u,
				1641u,
				1641u,
				1641u,
				1641u
			},
			{
				1776u,
				1776u,
				1776u,
				1776u,
				1776u
			},
			{
				1777u,
				1777u,
				1777u,
				1777u,
				1777u
			},
			{
				1778u,
				1778u,
				1778u,
				1778u,
				1778u
			},
			{
				1779u,
				1779u,
				1779u,
				1779u,
				1779u
			},
			{
				1780u,
				1780u,
				1780u,
				1780u,
				1780u
			},
			{
				1781u,
				1781u,
				1781u,
				1781u,
				1781u
			},
			{
				1782u,
				1782u,
				1782u,
				1782u,
				1782u
			},
			{
				1783u,
				1783u,
				1783u,
				1783u,
				1783u
			},
			{
				1784u,
				1784u,
				1784u,
				1784u,
				1784u
			},
			{
				1785u,
				1785u,
				1785u,
				1785u,
				1785u
			},
			{
				48u,
				48u,
				48u,
				48u,
				48u
			},
			{
				49u,
				49u,
				49u,
				49u,
				49u
			},
			{
				50u,
				50u,
				50u,
				50u,
				50u
			},
			{
				51u,
				51u,
				51u,
				51u,
				51u
			},
			{
				52u,
				52u,
				52u,
				52u,
				52u
			},
			{
				53u,
				53u,
				53u,
				53u,
				53u
			},
			{
				54u,
				54u,
				54u,
				54u,
				54u
			},
			{
				55u,
				55u,
				55u,
				55u,
				55u
			},
			{
				56u,
				56u,
				56u,
				56u,
				56u
			},
			{
				57u,
				57u,
				57u,
				57u,
				57u
			},
			{
				58u,
				58u,
				58u,
				58u,
				58u
			},
			{
				1548u,
				1548u,
				1548u,
				1548u,
				1548u
			},
			{
				1549u,
				1549u,
				1549u,
				1549u,
				1549u
			},
			{
				1563u,
				1563u,
				1563u,
				1563u,
				1563u
			},
			{
				1567u,
				1567u,
				1567u,
				1567u,
				1567u
			},
			{
				1642u,
				1642u,
				1642u,
				1642u,
				1642u
			},
			{
				1643u,
				1643u,
				1643u,
				1643u,
				1643u
			},
			{
				1644u,
				1644u,
				1644u,
				1644u,
				1644u
			},
			{
				1645u,
				1645u,
				1645u,
				1645u,
				1645u
			},
			{
				1748u,
				1748u,
				1748u,
				1748u,
				1748u
			},
			{
				46u,
				46u,
				46u,
				46u,
				46u
			},
			{
				65008u,
				65008u,
				65008u,
				65008u,
				65008u
			},
			{
				65009u,
				65009u,
				65009u,
				65009u,
				65009u
			},
			{
				65010u,
				65010u,
				65010u,
				65010u,
				65010u
			},
			{
				65011u,
				65011u,
				65011u,
				65011u,
				65011u
			},
			{
				65012u,
				65012u,
				65012u,
				65012u,
				65012u
			},
			{
				65013u,
				65013u,
				65013u,
				65013u,
				65013u
			},
			{
				65014u,
				65014u,
				65014u,
				65014u,
				65014u
			},
			{
				65015u,
				65015u,
				65015u,
				65015u,
				65015u
			},
			{
				65016u,
				65016u,
				65016u,
				65016u,
				65016u
			},
			{
				65017u,
				65017u,
				65017u,
				65017u,
				65017u
			},
			{
				65018u,
				65018u,
				65018u,
				65018u,
				65018u
			},
			{
				65019u,
				65019u,
				65019u,
				65019u,
				65019u
			},
			{
				65020u,
				65020u,
				65020u,
				65020u,
				65020u
			},
			{
				65021u,
				65021u,
				65021u,
				65021u,
				65021u
			},
			{
				1757u,
				1757u,
				1757u,
				1757u,
				1757u
			},
			{
				1758u,
				1758u,
				1758u,
				1758u,
				1758u
			},
			{
				65001u,
				65001u,
				65001u,
				65001u,
				65001u
			},
			{
				1750u,
				1750u,
				1750u,
				1750u,
				1750u
			},
			{
				1751u,
				1751u,
				1751u,
				1751u,
				1751u
			},
			{
				1752u,
				1752u,
				1752u,
				1752u,
				1752u
			},
			{
				1753u,
				1753u,
				1753u,
				1753u,
				1753u
			},
			{
				1754u,
				1754u,
				1754u,
				1754u,
				1754u
			},
			{
				1756u,
				1756u,
				1756u,
				1756u,
				1756u
			},
			{
				1768u,
				1768u,
				1768u,
				1768u,
				1768u
			},
			{
				1762u,
				1762u,
				1762u,
				1762u,
				1762u
			},
			{
				1769u,
				1769u,
				1769u,
				1769u,
				1769u
			},
			{
				65269u,
				65269u,
				65270u,
				65269u,
				65270u
			},
			{
				65271u,
				65271u,
				65272u,
				65271u,
				65272u
			},
			{
				65273u,
				65273u,
				65274u,
				65273u,
				65274u
			},
			{
				65275u,
				65275u,
				65276u,
				65275u,
				65276u
			}
		};

		private static uint[] canConnect_IsolatedArray = new uint[5]
		{
			1575u,
			1570u,
			1571u,
			1572u,
			1573u
		};

		private static uint[] isolated_IsolatedArray = new uint[45]
		{
			1569u,
			33u,
			34u,
			58u,
			1548u,
			1563u,
			1567u,
			1748u,
			46u,
			1642u,
			1643u,
			1644u,
			1645u,
			1632u,
			1633u,
			1634u,
			1635u,
			1636u,
			1637u,
			1638u,
			1639u,
			1640u,
			1641u,
			1776u,
			1777u,
			1778u,
			1779u,
			1780u,
			1781u,
			1782u,
			1783u,
			1784u,
			1785u,
			48u,
			49u,
			50u,
			51u,
			52u,
			53u,
			54u,
			55u,
			56u,
			57u,
			60u,
			62u
		};

		private static uint[] isolatedCharsArray = new uint[65]
		{
			1569u,
			33u,
			34u,
			1570u,
			1571u,
			1572u,
			1573u,
			1575u,
			1577u,
			1583u,
			1584u,
			1585u,
			1586u,
			1608u,
			1649u,
			1650u,
			1672u,
			1676u,
			1677u,
			1678u,
			1681u,
			1688u,
			1722u,
			1728u,
			1733u,
			1734u,
			1735u,
			1736u,
			1737u,
			1739u,
			1746u,
			1747u,
			1749u,
			1632u,
			1633u,
			1634u,
			1635u,
			1636u,
			1637u,
			1638u,
			1639u,
			1640u,
			1641u,
			1776u,
			1777u,
			1778u,
			1779u,
			1780u,
			1781u,
			1782u,
			1783u,
			1784u,
			1785u,
			58u,
			1548u,
			1563u,
			1567u,
			1748u,
			46u,
			1642u,
			1643u,
			1644u,
			1645u,
			60u,
			62u
		};

		private static uint[,] latinNumsArray = new uint[10, 3]
		{
			{
				48u,
				1632u,
				1776u
			},
			{
				49u,
				1633u,
				1777u
			},
			{
				50u,
				1634u,
				1778u
			},
			{
				51u,
				1635u,
				1779u
			},
			{
				52u,
				1636u,
				1780u
			},
			{
				53u,
				1637u,
				1781u
			},
			{
				54u,
				1638u,
				1782u
			},
			{
				55u,
				1639u,
				1783u
			},
			{
				56u,
				1640u,
				1784u
			},
			{
				57u,
				1641u,
				1785u
			}
		};

		private static STATE CurrentState;

		private static STATE NextCharacterState;

		private static CASE CharacterCase;

		private static _NumCase NumCase;

		private static char[] tashkeelChars = new char[8]
		{
			Convert.ToChar(1611),
			Convert.ToChar(1612),
			Convert.ToChar(1613),
			Convert.ToChar(1614),
			Convert.ToChar(1615),
			Convert.ToChar(1616),
			Convert.ToChar(1617),
			Convert.ToChar(1618)
		};

		private static char[] EnglishNums = new char[10]
		{
			Convert.ToChar(48),
			Convert.ToChar(49),
			Convert.ToChar(50),
			Convert.ToChar(51),
			Convert.ToChar(52),
			Convert.ToChar(53),
			Convert.ToChar(54),
			Convert.ToChar(55),
			Convert.ToChar(56),
			Convert.ToChar(57)
		};
		/// <summary>
		/// 小括号
		/// </summary>
		private static char[] Parentheses = new char[2]
		{
			Convert.ToChar(40),//左括号
			Convert.ToChar(41),//右括号
		};

		/// <summary>
		/// (Internal function) Core conversion of Arabic text
		/// </summary>
		/// <param name="inputArray"></param>
		/// <param name="numFormat"></param>
		/// <returns></returns>
		public static string Correct(char[] inputArray, int numFormat = 0, bool useHinduNumbers=true)
		{
			NumCase = (_NumCase)numFormat;
			int num = 0;
			int num2 = 0;
			uint num3 = 0u;
			uint value = 1604u;
			bool flag = false;
			List<char> list = new List<char>(inputArray.Length);
			CharacterCase = CASE.ccUnicode;
			CurrentState = STATE.sInitial;
			NextCharacterState = STATE.sInitial;
			while (num2 < inputArray.Length)
			{
				char c = inputArray[num2];
				num = num2 + 1;
				if (num < inputArray.Length)
				{
					num3 = Convert.ToUInt32(inputArray[num]);
				}
				if (c == Convert.ToChar(value) && num < inputArray.Length)
				{
					switch (num3)
					{
					case 1570u:
						c = Convert.ToChar(65269);
						num2++;
						flag = true;
						break;
					case 1571u:
						c = Convert.ToChar(65271);
						num2++;
						flag = true;
						break;
					case 1573u:
						c = Convert.ToChar(65273);
						num2++;
						flag = true;
						break;
					case 1575u:
						c = Convert.ToChar(65275);
						num2++;
						flag = true;
						break;
					}
				}
				if (!useHinduNumbers &&  _Contains(EnglishNums, c))
				{
					c = MapNum(c, NumCase);
				}
				//翻转小括号
				if (_Contains(Parentheses, c))
                {
					c = MapParentheses(c);
                }
				CharacterCase = CASE.ccUnicode;
				switch (CurrentState)
				{
				case STATE.sInitial:
					if (num2 + 1 == inputArray.Length || num3 == 0)
					{
						CharacterCase = CASE.ccIsolated;
						CurrentState = STATE.sFinal;
					}
					else if (char.IsWhiteSpace(inputArray[num]))
					{
						CharacterCase = CASE.ccIsolated;
						NextCharacterState = STATE.sInitial;
					}
					else if (If_Isolated(c) || _Contains(isolated_IsolatedArray, num3))
					{
						CharacterCase = CASE.ccIsolated;
						NextCharacterState = STATE.sInitial;
					}
					else if (char.IsWhiteSpace(c))
					{
						CharacterCase = CASE.ccInitial;
						NextCharacterState = STATE.sInitial;
					}
					else if (_Contains(tashkeelChars, Convert.ToChar(num3)))
					{
						CharacterCase = CASE.ccInitial;
						NextCharacterState = STATE.sInitial;
					}
					else if (_Contains(tashkeelChars, Convert.ToChar(num3)))
					{
						CharacterCase = CASE.ccIsolated;
						NextCharacterState = STATE.sInitial;
					}
					else if (_Contains(canConnect_IsolatedArray, num3))
					{
						CharacterCase = CASE.ccInitial;
						NextCharacterState = STATE.sMedial;
					}
					else
					{
						CharacterCase = CASE.ccInitial;
						NextCharacterState = STATE.sMedial;
					}
					if (flag)
					{
						NextCharacterState = STATE.sInitial;
					}
					break;
				case STATE.sMedial:
					if (num2 + 1 == inputArray.Length || num3 == 0)
					{
						CharacterCase = CASE.ccFinal;
						CurrentState = STATE.sFinal;
					}
					else if (char.IsWhiteSpace(inputArray[num]))
					{
						CharacterCase = CASE.ccFinal;
						NextCharacterState = STATE.sInitial;
					}
					else if (If_Isolated(c) || _Contains(isolated_IsolatedArray, num3))
					{
						CharacterCase = CASE.ccFinal;
						NextCharacterState = STATE.sInitial;
					}
					else
					{
						CharacterCase = CASE.ccMedial;
						NextCharacterState = STATE.sMedial;
					}
					if (flag)
					{
						NextCharacterState = STATE.sInitial;
					}
					break;
				}
				flag = false;
				list.Add(MapChar(c, CharacterCase));
				num2++;
				if (CurrentState == STATE.sFinal)
				{
					break;
				}
				CurrentState = NextCharacterState;
			}
			return new string(list.ToArray());
		}

		/// <summary>
		/// Remap Arabic/Persian numbers back to Latin
		/// </summary>
		/// <param name="text">text that contains numbers only</param>
		/// <returns></returns>
		public static string RemapNumsToLatin(string text)
		{
			char[] array = text.ToCharArray();
			char[] array2 = new char[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if ((array[i] >= '٠' && array[i] <= '٩') || (array[i] >= '۰' && array[i] <= '۹'))
				{
					array2[i] = RemapToLatin(array[i]);
				}
				else
				{
					array2[i] = array[i];
				}
			}
			return new string(array2);
		}

		private static char MapChar(char unicodeCharacter, CASE currCharCase)
		{
			char result = '\0';
			uint num = Convert.ToUInt32(unicodeCharacter);
			if (unicodeCharacter < '\u0600' || unicodeCharacter > 'ۿ')
			{
				return unicodeCharacter;
			}
			for (int i = 0; i < arabicCharsArray.Length; i++)
			{
				if (arabicCharsArray[i, 0] == num)
				{
					return result = Convert.ToChar(arabicCharsArray[i, (int)currCharCase]);
				}
			}
			return result;
		}

		private static char MapNum(char latinNum, _NumCase numCase)
		{
			uint num = Convert.ToUInt32(latinNum);
			for (int i = 0; i < latinNumsArray.Length / 3; i++)
			{
				if (latinNumsArray[i, 0] == num)
				{
					return latinNum = Convert.ToChar(latinNumsArray[i, (int)numCase]);
				}
			}
			return latinNum;
		}
		private static char MapParentheses(char c)
        {
			if (c == Parentheses[0])
            {
				return Parentheses[1];
            }
            if (c == Parentheses[1])
            {
				return Parentheses[0];
            }
			return c;
        }



		private static char RemapToLatin(char arabicNum)
		{
			uint num = Convert.ToUInt32(arabicNum);
			for (int i = 0; i < latinNumsArray.Length / 3; i++)
			{
				if (latinNumsArray[i, 1] == num || latinNumsArray[i, 2] == num)
				{
					return arabicNum = Convert.ToChar(latinNumsArray[i, 0]);
				}
			}
			return arabicNum;
		}

		private static bool If_Isolated(char isolatedChar)
		{
			uint num = Convert.ToUInt32(isolatedChar);
			for (int i = 0; i < isolatedCharsArray.Length; i++)
			{
				if (isolatedCharsArray[i] == num)
				{
					return true;
				}
			}
			return false;
		}

		private static bool _Contains(char[] arr, char character)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] == character)
				{
					return true;
				}
			}
			return false;
		}

		private static bool _Contains(uint[] arr, uint character)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] == character)
				{
					return true;
				}
			}
			return false;
		}
	}
}
