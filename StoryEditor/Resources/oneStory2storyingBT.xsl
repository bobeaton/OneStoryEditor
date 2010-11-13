<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE stylesheet [
<!ENTITY cr "&#xD;&#xA;">
<!ENTITY tab "&#9;">
<!ENTITY nbsp "&#160;">
]>
<xsl:transform version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
               xmlns:msxsl="urn:schemas-microsoft-com:xslt"
               xmlns:user="urn:my-scripts">
  <msxsl:script language="C#" implements-prefix="user">
    <msxsl:assembly name="System.Windows.Forms" />
    <![CDATA[
  public string Rtf2Text(string strRtf)
  {
    System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
    rtBox.Rtf = strRtf;
    return rtBox.Text;
  }
  public class BookMap : System.Collections.Generic.Dictionary<string,string>
  {
    public BookMap()
    {
      Add("Gen", "Gen");
      Add("Exod", "Exo");
      Add("Lev", "Lev");
      Add("Num", "Num");
      Add("Deut", "Deu");
      Add("Josh", "Jos");
      Add("Judg", "Jdg");
      Add("Ruth", "Rut");
      Add("1Sam", "1Sa");
      Add("2Sam", "2Sa");
      Add("1Kgs", "1Ki");
      Add("2Kgs", "2Ki");
      Add("1Chr", "1Ch");
      Add("2Chr", "2Ch");
      Add("Ezra", "Ezr");
      Add("Neh", "Neh");
      Add("Esth", "Est");
      Add("Job", "Job");
      Add("Ps", "Psa");
      Add("Prov", "Pro");
      Add("Eccl", "Ecc");
      Add("Song", "Sng");
      Add("Isa", "Isa");
      Add("Jer", "Jer");
      Add("Lam", "Lam");
      Add("Ezek", "Ezk");
      Add("Dan", "Dan");
      Add("Hos", "Hos");
      Add("Joel", "Jol");
      Add("Amos", "Amo");
      Add("Obad", "Oba");
      Add("Jonah", "Jon");
      Add("Mic", "Mic");
      Add("Nah", "Nam");
      Add("Hab", "Hab");
      Add("Zeph", "Zep");
      Add("Hag", "Hag");
      Add("Zech", "Zec");
      Add("Mal", "Mal");
      Add("Matt", "Mat");
      Add("Mark", "Mrk");
      Add("Luke", "Luk");
      Add("John", "Jhn");
      Add("Acts", "Act");
      Add("Rom", "Rom");
      Add("1Cor", "1Co");
      Add("2Cor", "2Co");
      Add("Gal", "Gal");
      Add("Eph", "Eph");
      Add("Phil", "Php");
      Add("Col", "Col");
      Add("1Thess", "1Th");
      Add("2Thess", "2Th");
      Add("1Tim", "1Ti");
      Add("2Tim", "2Ti");
      Add("Titus", "Tit");
      Add("Phlm", "Phm");
      Add("Heb", "Heb");
      Add("Jas", "Jas");
      Add("1Pet", "1Pe");
      Add("2Pet", "2Pe");
      Add("1John", "1Jn");
      Add("2John", "2Jn");
      Add("3John", "3Jn");
      Add("Jude", "Jud");
      Add("Rev", "Rev");
    }
  }
  BookMap map = new BookMap();
  public string ConvertToToolboxAnchor(string strAnchor)
  {
    int nIndexSpace = strAnchor.IndexOf(' ');
    string strBookCode = strAnchor.Substring(0, nIndexSpace);
    try
    {
      strBookCode = map[strBookCode];
    }
    catch
    {
      // ignore
    }
    string[] astrChVrs = strAnchor.Substring(nIndexSpace+1).Split(new char[] { ':' });
    int nLen = 2;
    string strChap = astrChVrs[0];
    string strVerse = astrChVrs[1];
    while (strVerse.Length < nLen)
      strVerse = "0" + strVerse;
    if (strBookCode == "Psa")
      nLen++;
    while (strChap.Length < nLen)
      strChap = "0" + strChap;
    return strBookCode + "." + strChap + ":" + strVerse;
  }
  ]]>
  </msxsl:script>

  <xsl:param name="storySet"/>

  <xsl:output method="text" encoding="UTF-8" indent="no"/>
    <xsl:strip-space elements="*"/>

    <!-- oneStory2storyingBT.xsl         
        create by Jim Albright to handle conversion of onestory to sfm
        2010-04-03
    -->    
    
    <xsl:template match="StoryProject">
      <xsl:text>\_sh v3.0  374  StoryBT</xsl:text>
      <xsl:text>&cr;&cr;\c 00</xsl:text>
      <xsl:text>&cr;\t </xsl:text>
      <xsl:value-of select="@ProjectName"/>
      <xsl:text>&cr;\co Language: </xsl:text>
      <xsl:value-of select="preceding::Languages/VernacularLang/@name"/>
      <xsl:variable name="RtfFrontMatter" select="@PanoramaFrontMatter"></xsl:variable>
      <xsl:text>&cr;&cr;\co Front Matter:&cr;</xsl:text>
      <xsl:value-of select="user:Rtf2Text($RtfFrontMatter)"/>
      <xsl:text>&cr;&cr;\co Team Members:</xsl:text>
      <xsl:apply-templates select="Members"/>
      <xsl:apply-templates select="stories"/>
    </xsl:template>

  <xsl:template match="Members">
    <xsl:apply-templates select="Member"/>
  </xsl:template>

  <xsl:template match="Member[not(@name='Browser')]">
    <xsl:text>&cr;\co  </xsl:text>
    <xsl:value-of select="@name"/>
    <xsl:text> (</xsl:text>
    <xsl:value-of select="@memberType"/>
    <xsl:text>): </xsl:text>
    <xsl:value-of select="@bioData"/>
  </xsl:template>

  <xsl:template match="stories">
    <xsl:if test="@SetName = $storySet">
      <xsl:apply-templates select="story"/>
    </xsl:if>
  </xsl:template>         

<xsl:template match="story">
  <xsl:text>&cr;&cr;\c </xsl:text>
  <xsl:number value="position()" format="01" />
  <xsl:text>&cr;\t </xsl:text><xsl:value-of select="@name"/>
  <xsl:text>&cr;\co </xsl:text><xsl:value-of select="preceding::Languages/VernacularLang/@name"/>
  <xsl:variable name="projectFacilitator" select="descendant::ProjectFacilitator/@memberID"/>
  <xsl:text>&cr;&cr;\co Project Facilitator: </xsl:text>
  <xsl:value-of select="preceding::Member[@memberKey=$projectFacilitator]/@name"/>
  <xsl:variable name="storyCrafter" select="descendant::StoryCrafter/@memberID"/>
  <xsl:text>&cr;\co Story crafter: </xsl:text><xsl:value-of select="preceding::Member[@memberKey=$storyCrafter]/@name"/>
  <xsl:text>&cr;\co Resources used: </xsl:text>
  <xsl:value-of select="descendant::ResourcesUsed"/>
  <xsl:variable name="BackTranslator" select="descendant::BackTranslator/@memberID"/>
  <xsl:text>&cr;\co Backtranslator to </xsl:text><xsl:value-of select="preceding::Languages/NationalBTLang/@name"/><xsl:text>: </xsl:text><xsl:value-of select="preceding::Member[@memberKey=$BackTranslator]/@name"/> 
  <xsl:text>&cr;\co Backtranslator </xsl:text><xsl:value-of select="preceding::Languages/NationalBTLang/@name"></xsl:value-of><xsl:text> to </xsl:text><xsl:value-of select="preceding::Languages/InternationalBTLang/@name"/><xsl:text>: </xsl:text>
  <xsl:text>&cr;\co reason this story is in the set: </xsl:text><xsl:value-of select="descendant::StoryPurpose"/>
  <xsl:variable name="stageId" select="concat('e', @stage)"/>
  <xsl:text>&cr;\co Story State: </xsl:text>
  <xsl:value-of select="document('{0}\StageTransitions.xml')/ProjectStates/StateTransition[@stage = $stageId]/StageDisplayString"/>
  <xsl:apply-templates select="descendant::Test"/>
  <xsl:text>&cr;</xsl:text>
  <xsl:apply-templates select="descendant::verses">
    <xsl:with-param name="chapNo" select="position()" />
  </xsl:apply-templates>
</xsl:template> 
   
<xsl:template match="Test">
    <xsl:text>&cr;\co Testing </xsl:text>
    <xsl:value-of select="count(preceding-sibling::Test)+1"></xsl:value-of>
    <xsl:variable name="Testing" select="@memberID"/>
    <xsl:text>: </xsl:text>   
    <xsl:value-of select="preceding::Member[@memberKey=$Testing]/@name"/>
</xsl:template>

    <xsl:template match="verses">
      <xsl:param name="chapNo" />
      <xsl:apply-templates>
        <xsl:with-param name="chapNo" select="$chapNo" />
      </xsl:apply-templates>
    </xsl:template>
    <xsl:template match="verse[not(@first)]">
      <xsl:param name="chapNo" />
      <xsl:text>&cr;\ln </xsl:text><xsl:number value="$chapNo" format="01" /><xsl:text>.</xsl:text><xsl:number value="position()-1" format="01" />
      <xsl:if test="@visible = 'false'">
        <xsl:text> (hidden)</xsl:text>
      </xsl:if>
      <xsl:if test="Vernacular[text()] and preceding::Languages/VernacularLang[@code]">
        <xsl:text>&cr;\</xsl:text>
        <xsl:value-of select="preceding::Languages/VernacularLang/@code"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="descendant::Vernacular"/>
      </xsl:if>
      <xsl:if test="NationalBT[text()] and preceding::Languages/NationalBTLang[@code]">
        <xsl:text>&cr;\bt</xsl:text>
        <xsl:value-of select="preceding::Languages/NationalBTLang/@code"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="descendant::NationalBT"/>
      </xsl:if>
      <xsl:if test="InternationalBT[text()] and preceding::Languages/InternationalBTLang[@code]">
        <xsl:text>&cr;\bt</xsl:text>
        <xsl:value-of select="preceding::Languages/InternationalBTLang/@code"/>
        <xsl:text> </xsl:text>
        <xsl:value-of select="descendant::InternationalBT"/>
      </xsl:if>
      <xsl:apply-templates select="anchors"/>
      <xsl:apply-templates select="TestQuestions" />
    </xsl:template>

  <xsl:template match="anchors">
    <xsl:text>&cr;\anc </xsl:text>
    <xsl:for-each select="anchor">
      <xsl:variable name="ancJT" select="@jumpTarget"/>
      <xsl:value-of select="user:ConvertToToolboxAnchor($ancJT)"/>
      <xsl:if test="position()>0 and position()!=last()">
        <xsl:text>; </xsl:text>
      </xsl:if>
    </xsl:for-each>
    <xsl:apply-templates select="descendant::exegeticalHelps"/>
  </xsl:template>

  <xsl:template match="exegeticalHelps">
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="exegeticalHelp">
    <xsl:text>&cr;\cn </xsl:text>
    <xsl:value-of select="node()"/>
  </xsl:template>

  <xsl:template match="TestQuestions">
      <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="TestQuestion">
    <xsl:if test="TQVernacular[text()] and preceding::Languages/VernacularLang[@code]">
      <xsl:text>&cr;\tst</xsl:text>
      <xsl:value-of select="preceding::Languages/VernacularLang/@code"/>
      <xsl:text> </xsl:text>
      <xsl:value-of select="TQVernacular"/>
    </xsl:if>
    <xsl:if test="TQInternationalBT[text()] and preceding::Languages/InternationalBTLang[@code]">
      <xsl:text>&cr;\tst</xsl:text>
      <xsl:value-of select="preceding::Languages/InternationalBTLang/@code"/>
      <xsl:text> </xsl:text>
      <xsl:value-of select="TQInternationalBT"/>
    </xsl:if>
    <xsl:apply-templates select="Answers"/>
  </xsl:template>

  <xsl:template match="Answers">
      <xsl:apply-templates/>
  </xsl:template>
  
  <xsl:template match="answer">
    <xsl:text>&cr;\ans T</xsl:text>
    <xsl:number value="position()" format="1"/>
    <xsl:text>: </xsl:text>
    <xsl:value-of select="."/>
  </xsl:template>
  
  <!-- toss out anything else -->
  <xsl:template match="*"/> 
     
</xsl:transform>









