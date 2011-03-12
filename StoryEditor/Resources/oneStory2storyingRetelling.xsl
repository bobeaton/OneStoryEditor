<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE stylesheet [
<!ENTITY cr "&#xD;&#xA;">
<!ENTITY tab "&#9;">
<!ENTITY nbsp "&#160;">
]>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
    <xsl:output method="text" encoding="UTF-8" indent="no"/>
    <xsl:strip-space elements="*"/>

    <!-- oneStory2storyingRetelling.xsl
        create by Bob Eaton based on Jim Albright's script for storyingBT 
        (to handle conversion of onestory to sfm)
        2010-04-05
    -->    
    
    <xsl:template match="StoryProject">
        <xsl:text>\_sh v3.0  374  Retellings</xsl:text>
        <xsl:apply-templates select="stories"/>
    </xsl:template>         

    <xsl:template match="stories[@SetName='Stories']">
        <xsl:apply-templates select="story"/>
    </xsl:template>         
    
<xsl:template match="story">
  <xsl:variable name="chapNo" select="position()"/>
  <xsl:text>&cr;&cr;\c </xsl:text>
  <xsl:number value="$chapNo" format="01" />
  <xsl:text>&cr;\t </xsl:text><xsl:value-of select="@name"/>
  <xsl:text>&cr;\co </xsl:text><xsl:value-of select="preceding::Languages/LanguageInfo[@lang='Vernacular']/@name"/>

  <xsl:for-each select="CraftingInfo/TestsRetellings/TestRetelling">
    <xsl:text>&cr;&cr;\co Test </xsl:text>
    <xsl:value-of select="count(preceding-sibling::TestRetelling)+1"></xsl:value-of>
    <xsl:variable name="TestorID" select="@memberID"/>
    <xsl:text>: </xsl:text>
    <xsl:value-of select="preceding::Member[@memberKey=$TestorID]/@name"/>
    <xsl:apply-templates select="ancestor::story/Verses">
      <xsl:with-param name="chapNo" select="$chapNo" />
      <xsl:with-param name="testorId" select="$TestorID" />
    </xsl:apply-templates>
  </xsl:for-each>

  <xsl:for-each select="CraftingInfo/TestsTqAnswers/TestTqAnswer">
    <xsl:text>&cr;&cr;\co Test </xsl:text>
    <xsl:value-of select="count(preceding-sibling::TestTqAnswer)+1"></xsl:value-of>
    <xsl:variable name="TestorID" select="@memberID"/>
    <xsl:text>: </xsl:text>
    <xsl:value-of select="preceding::Member[@memberKey=$TestorID]/@name"/>
    <xsl:apply-templates select="ancestor::story/Verses">
      <xsl:with-param name="chapNo" select="$chapNo" />
      <xsl:with-param name="testorId" select="$TestorID" />
    </xsl:apply-templates>
  </xsl:for-each>
</xsl:template> 
   
    <xsl:template match="Verses">
      <xsl:param name="chapNo" />
      <xsl:param name="testorId" />
      <xsl:apply-templates>
        <xsl:with-param name="chapNo" select="$chapNo" />
        <xsl:with-param name="testorId" select="$testorId" />
      </xsl:apply-templates>
    </xsl:template>
    <xsl:template match="Verse[not(@first)]">
      <xsl:param name="chapNo" />
      <xsl:param name="testorId" />
      <xsl:text>&cr;\ln </xsl:text>
      <xsl:number value="$chapNo" format="01" />
      <xsl:text>.</xsl:text>
      <xsl:number value="position()-1" format="01" />
      <xsl:if test="Retellings/Retelling[@memberID=$testorId]">
        <xsl:text>&cr;\ret </xsl:text>
        <xsl:value-of select="Retellings/Retelling[@lang='InternationalBt'][@memberID=$testorId]"/>
      </xsl:if>
    </xsl:template>

    <!-- toss out anything else -->
    <xsl:template match="*"/> 
     
</xsl:transform>









