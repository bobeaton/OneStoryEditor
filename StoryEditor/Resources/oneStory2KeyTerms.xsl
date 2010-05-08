<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE stylesheet [
<!ENTITY cr "&#xD;&#xA;">
<!ENTITY tab "&#9;">
<!ENTITY nbsp "&#160;">
]>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="2.0">
    <xsl:output method="text" encoding="UTF-8" indent="no"/>
    <xsl:strip-space elements="*"/>

    <!-- oneStory2KeyTerms.xsl
        create by Bob Eaton based on Jim Albright's script for storyingBT 
        (to handle conversion of onestory to sfm)
        2010-04-05
        key terms based on L&C Notes.txt:
    -->

  <xsl:template match="TermRenderingsList">
    <xsl:text>\_sh v3.0  374  LnC Note&cr;</xsl:text>
    <xsl:apply-templates select="Renderings"/>
  </xsl:template>

  <xsl:template match="Renderings">
    <xsl:for-each select="TermRendering[Renderings]">
      <xsl:if test="Renderings[text()]">
        <xsl:variable name="KTId" select="Id"/>
        <xsl:text>&cr;\r </xsl:text>
        <xsl:value-of select="document('{0}\BiblicalTerms\BiblicalTermsEn.xml')/BiblicalTermsLocalizations/Terms/Localization[@Id = $KTId]/@Gloss"/>
        <xsl:text>&cr;\wrd </xsl:text>
        <xsl:value-of select="Renderings"/>
        <xsl:text>&cr;\co </xsl:text>
        <xsl:value-of select="Notes"/>
        <xsl:text>&cr;</xsl:text>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>         

  <!-- toss out anything else -->
  <xsl:template match="*"/> 
     
</xsl:transform>
