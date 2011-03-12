<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE stylesheet [
<!ENTITY cr "&#xD;&#xA;">
<!ENTITY tab "&#9;">
<!ENTITY nbsp "&#160;">
]>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
    <xsl:output method="text" encoding="UTF-8" indent="no"/>
    <xsl:strip-space elements="*"/>

    <!-- oneStory2ConNotes.xsl         
        create by Bob Eaton based on Jim Albright's script for storyingBT 
        (to handle conversion of onestory to sfm)
        2010-04-05
    -->    
    
    <xsl:template match="StoryProject">
        <xsl:text>\_sh v3.0  374  StoryNotes</xsl:text>
        <xsl:apply-templates select="stories"/>
    </xsl:template>         

    <xsl:template match="stories[@SetName='Stories']">
        <xsl:apply-templates select="story"/>
    </xsl:template>         
    

<xsl:template match="story">
  <xsl:text>&cr;&cr;\c </xsl:text>
  <xsl:number value="position()" format="01" />
  <xsl:text>&cr;\t </xsl:text><xsl:value-of select="@name"/>
  <xsl:text>&cr;\co </xsl:text><xsl:value-of select="preceding::Languages/LanguageInfo[@lang='Vernacular']/@name"/>
  <xsl:text>&cr;</xsl:text>
  <xsl:apply-templates select="descendant::Verses">
    <xsl:with-param name="chapNo" select="position()" />
  </xsl:apply-templates>
</xsl:template> 
   
    <xsl:template match="Verses">
      <xsl:param name="chapNo" />
      <xsl:apply-templates>
        <xsl:with-param name="chapNo" select="$chapNo" />
      </xsl:apply-templates>
    </xsl:template>
    <xsl:template match="Verse">
      <xsl:param name="chapNo" />
      <xsl:text>&cr;\ln </xsl:text><xsl:number value="$chapNo" format="01" /><xsl:text>.</xsl:text><xsl:number value="position()-1" format="01" />
      <xsl:apply-templates select="descendant::ConsultantConversation"/>
    </xsl:template>

  <xsl:template match="ConsultantConversation">
    <xsl:text>&cr;\rnd </xsl:text><xsl:value-of select="@round"/>
    <xsl:apply-templates select="ConsultantNote"/>
  </xsl:template>

  <xsl:template match="ConsultantNote">
    <xsl:choose>
      <xsl:when test="@Direction='ConsultantToProjFac'">
        <xsl:text>&cr;\con </xsl:text>
        <xsl:value-of select="."/>
      </xsl:when>
      <xsl:when test="@Direction='ConsultantToConsultant'">
        <xsl:text>&cr;\con2self </xsl:text>
        <xsl:value-of select="."/>
      </xsl:when>
      <xsl:when test="@Direction='ProjFacToConsultant'">
        <xsl:text>&cr;\fac </xsl:text>
        <xsl:value-of select="."/>
      </xsl:when>
    </xsl:choose>
  </xsl:template>

    <!-- toss out anything else -->
    <xsl:template match="*"/> 
     
</xsl:transform>









